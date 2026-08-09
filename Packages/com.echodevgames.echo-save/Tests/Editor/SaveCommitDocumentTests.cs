
using System;
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveCommitDocumentTests
    {
        private UnityJsonSaveSerializer serializer;
        private Sha256IntegrityProvider integrity;
        private SaveSlotId slotId;
        private SaveGenerationId generationId;

        [SetUp]
        public void SetUp()
        {
            serializer =
                new UnityJsonSaveSerializer();

            integrity =
                new Sha256IntegrityProvider();

            slotId =
                SaveSlotId.NewId();

            generationId =
                SaveGenerationId.NewId();
        }

        [Test]
        public void EmptyPayloadManifestAgreementPasses()
        {
            CreateValidPair(
                out SaveManifest manifest,
                out SavePayloadDocument payload,
                out byte[] payloadBytes);

            SaveDocumentValidationResult result =
                SaveCommitDocumentValidator
                    .ValidateManifestAndPayload(
                        manifest,
                        payload,
                        payloadBytes,
                        integrity);

            Assert.That(
                result.Succeeded,
                Is.True);
        }

        [Test]
        public void MismatchedSlotIdentityFails()
        {
            CreateValidPair(
                out SaveManifest manifest,
                out SavePayloadDocument payload,
                out byte[] payloadBytes);

            payload.slotId =
                SaveSlotId.NewId()
                    .Value;

            SaveDocumentValidationResult result =
                SaveCommitDocumentValidator
                    .ValidateManifestAndPayload(
                        manifest,
                        payload,
                        payloadBytes,
                        integrity);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveDocumentValidationStatus
                        .IdentityMismatch));
        }

        [Test]
        public void MismatchedGenerationIdentityFails()
        {
            CreateValidPair(
                out SaveManifest manifest,
                out SavePayloadDocument payload,
                out byte[] payloadBytes);

            payload.generationId =
                SaveGenerationId.NewId()
                    .Value;

            SaveDocumentValidationResult result =
                SaveCommitDocumentValidator
                    .ValidateManifestAndPayload(
                        manifest,
                        payload,
                        payloadBytes,
                        integrity);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveDocumentValidationStatus
                        .IdentityMismatch));
        }

        [Test]
        public void PayloadByteLengthMismatchFails()
        {
            CreateValidPair(
                out SaveManifest manifest,
                out SavePayloadDocument payload,
                out byte[] payloadBytes);

            manifest.payloadByteLength++;

            SaveDocumentValidationResult result =
                SaveCommitDocumentValidator
                    .ValidateManifestAndPayload(
                        manifest,
                        payload,
                        payloadBytes,
                        integrity);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveDocumentValidationStatus
                        .PayloadLengthMismatch));
        }

        [Test]
        public void PayloadChecksumMismatchFails()
        {
            CreateValidPair(
                out SaveManifest manifest,
                out SavePayloadDocument payload,
                out byte[] payloadBytes);

            manifest.payloadChecksum =
                new string(
                    '0',
                    64);

            SaveDocumentValidationResult result =
                SaveCommitDocumentValidator
                    .ValidateManifestAndPayload(
                        manifest,
                        payload,
                        payloadBytes,
                        integrity);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveDocumentValidationStatus
                        .IntegrityMismatch));
        }

        [Test]
        public void UnsupportedIntegrityProviderFails()
        {
            CreateValidPair(
                out SaveManifest manifest,
                out SavePayloadDocument payload,
                out byte[] payloadBytes);

            manifest.integrityAlgorithm =
                "tests.other";

            SaveDocumentValidationResult result =
                SaveCommitDocumentValidator
                    .ValidateManifestAndPayload(
                        manifest,
                        payload,
                        payloadBytes,
                        integrity);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveDocumentValidationStatus
                        .UnsupportedIntegrityProvider));
        }

        [Test]
        public void InventoryMismatchFails()
        {
            SavePayloadEntry payloadEntry =
                new SavePayloadEntry
                {
                    participantId =
                        "com.example.inventory",
                    participantSchemaVersion = 3,
                    serializerId =
                        UnityJsonSaveSerializer.StableId,
                    required = true,
                    byteLength = 10,
                    checksum =
                        new string(
                            'a',
                            64)
                };

            SavePayloadDocument payload =
                new SavePayloadDocument
                {
                    slotId = slotId.Value,
                    generationId =
                        generationId.Value,
                    entries =
                        new[] { payloadEntry }
                };

            serializer.Serialize(
                payload,
                out string payloadJson);

            byte[] payloadBytes =
                Encoding.UTF8.GetBytes(
                    payloadJson);

            integrity.Calculate(
                payloadBytes,
                out string checksum);

            SaveManifest manifest =
                new SaveManifest
                {
                    slotId = slotId.Value,
                    generationId =
                        generationId.Value,
                    payloadByteLength =
                        payloadBytes.LongLength,
                    payloadChecksum =
                        checksum,
                    payloadEntries =
                        Array.Empty<
                            SavePayloadInventoryEntry>()
                };

            SaveDocumentValidationResult result =
                SaveCommitDocumentValidator
                    .ValidateManifestAndPayload(
                        manifest,
                        payload,
                        payloadBytes,
                        integrity);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveDocumentValidationStatus
                        .InventoryMismatch));
        }

        [Test]
        public void ManifestPayloadAndHeadRoundTrip()
        {
            CreateValidPair(
                out SaveManifest manifest,
                out SavePayloadDocument payload,
                out _);

            SaveHeadPointer head =
                new SaveHeadPointer
                {
                    slotId = slotId.Value,
                    currentGenerationId =
                        generationId.Value,
                    updateSequence = 1
                };

            AssertRoundTrip(
                manifest);

            AssertRoundTrip(
                payload);

            AssertRoundTrip(
                head);
        }

        [Test]
        public void HeadWithCurrentGenerationIsValid()
        {
            SaveHeadPointer head =
                new SaveHeadPointer
                {
                    slotId = slotId.Value,
                    currentGenerationId =
                        generationId.Value,
                    updateSequence = 1
                };

            SaveDocumentValidationResult result =
                SaveCommitDocumentValidator
                    .ValidateHead(
                        head);

            Assert.That(
                result.Succeeded,
                Is.True);
        }

        [Test]
        public void HeadWithUnsupportedVersionFails()
        {
            SaveHeadPointer head =
                new SaveHeadPointer
                {
                    slotId = slotId.Value,
                    currentGenerationId =
                        generationId.Value,
                    formatMajor =
                        SaveDocumentVersions
                            .HeadPointerMajor + 1
                };

            SaveDocumentValidationResult result =
                SaveCommitDocumentValidator
                    .ValidateHead(
                        head);

            Assert.That(
                result.Succeeded,
                Is.False);
        }

        [Test]
        public void EmptyPayloadDocumentRoundTrips()
        {
            SavePayloadDocument payload =
                new SavePayloadDocument
                {
                    slotId = slotId.Value,
                    generationId =
                        generationId.Value,
                    entries =
                        Array.Empty<
                            SavePayloadEntry>()
                };

            SaveSerializerResult serialized =
                serializer.Serialize(
                    payload,
                    out string json);

            SaveSerializerResult deserialized =
                serializer.Deserialize(
                    json,
                    out SavePayloadDocument restored);

            Assert.That(
                serialized.Succeeded,
                Is.True);

            Assert.That(
                deserialized.Succeeded,
                Is.True);

            Assert.That(
                restored.entries,
                Is.Empty);
        }

        private void CreateValidPair(
            out SaveManifest manifest,
            out SavePayloadDocument payload,
            out byte[] payloadBytes)
        {
            payload =
                new SavePayloadDocument
                {
                    slotId = slotId.Value,
                    generationId =
                        generationId.Value,
                    entries =
                        Array.Empty<
                            SavePayloadEntry>()
                };

            SaveSerializerResult serialized =
                serializer.Serialize(
                    payload,
                    out string payloadJson);

            Assert.That(
                serialized.Succeeded,
                Is.True);

            payloadBytes =
                Encoding.UTF8.GetBytes(
                    payloadJson);

            SaveIntegrityResult hashed =
                integrity.Calculate(
                    payloadBytes,
                    out string checksum);

            Assert.That(
                hashed.Succeeded,
                Is.True);

            manifest =
                new SaveManifest
                {
                    slotId = slotId.Value,
                    generationId =
                        generationId.Value,
                    createdUtc =
                        "2026-08-09T16:53:00Z",
                    updatedUtc =
                        "2026-08-09T16:53:00Z",
                    saveKind =
                        "manual",
                    projectId =
                        "com.example.game",
                    projectVersion =
                        "1.0.0",
                    buildId =
                        "test-build",
                    displayName =
                        "Test Save",
                    payloadByteLength =
                        payloadBytes.LongLength,
                    payloadChecksum =
                        checksum,
                    integrityAlgorithm =
                        Sha256IntegrityProvider
                            .StableId,
                    payloadEntries =
                        Array.Empty<
                            SavePayloadInventoryEntry>(),
                    commitState =
                        SaveGenerationCommitState
                            .Verified
                };
        }

        private void AssertRoundTrip<T>(
            T source)
        {
            SaveSerializerResult serialized =
                serializer.Serialize(
                    source,
                    out string json);

            SaveSerializerResult deserialized =
                serializer.Deserialize(
                    json,
                    out T restored);

            Assert.That(
                serialized.Succeeded,
                Is.True);

            Assert.That(
                deserialized.Succeeded,
                Is.True);

            Assert.That(
                restored,
                Is.Not.Null);
        }
    }
}
