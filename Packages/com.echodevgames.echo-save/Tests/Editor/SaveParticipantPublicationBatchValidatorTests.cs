
using System;
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantPublicationBatchValidatorTests
    {
        private Sha256IntegrityProvider integrity;

        [SetUp]
        public void SetUp()
        {
            integrity =
                new Sha256IntegrityProvider();
        }

        [Test]
        public void ValidCaptureBatchCrossesPublicationBoundary()
        {
            SaveParticipantCaptureBatchResult batch =
                CreateBatch(
                    Entry(
                        "com.example.inventory",
                        "{\"gold\":100}"));

            SaveDocumentValidationResult result =
                SaveParticipantPublicationBatchValidator
                    .ValidateCaptureBatch(
                        batch,
                        integrity,
                        out SavePayloadEntry[] payload,
                        out SavePayloadInventoryEntry[] inventory);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                payload.Length,
                Is.EqualTo(1));

            Assert.That(
                inventory.Length,
                Is.EqualTo(1));
        }

        [Test]
        public void FailedCaptureBatchIsRejected()
        {
            SaveParticipantCaptureBatchResult batch =
                SaveParticipantCaptureBatchResult
                    .Failure(
                        SaveParticipantCaptureBatchStatus
                            .CaptureFailed,
                        new SaveParticipantId(
                            "com.example.inventory"),
                        "ESV-TEST",
                        "Injected failure.");

            AssertRejected(
                batch);
        }

        [Test]
        public void EmptySuccessfulBatchIsRejectedForParticipantPublication()
        {
            SaveParticipantCaptureBatchResult batch =
                SaveParticipantCaptureBatchResult
                    .Success(
                        Array.Empty<SavePayloadEntry>(),
                        Array.Empty<
                            SavePayloadInventoryEntry>(),
                        0L);

            AssertRejected(
                batch);
        }

        [Test]
        public void DuplicateParticipantIdsAreRejected()
        {
            EntryPair first =
                Entry(
                    "com.example.inventory",
                    "{\"gold\":100}");

            EntryPair duplicate =
                Entry(
                    "com.example.inventory",
                    "{\"gold\":200}");

            AssertRejected(
                CreateBatch(
                    first,
                    duplicate));
        }

        [Test]
        public void NonCanonicalParticipantOrderIsRejected()
        {
            AssertRejected(
                CreateBatch(
                    Entry(
                        "com.example.zeta",
                        "{\"value\":1}"),
                    Entry(
                        "com.example.alpha",
                        "{\"value\":2}")));
        }

        [Test]
        public void InvalidParticipantIdIsRejected()
        {
            EntryPair pair =
                Entry(
                    "com.example.inventory",
                    "{\"gold\":100}");

            pair.Payload.participantId =
                "Inventory";

            pair.Inventory.participantId =
                "Inventory";

            AssertRejected(
                CreateBatch(
                    pair));
        }

        [Test]
        public void NonPositiveSchemaVersionIsRejected()
        {
            EntryPair pair =
                Entry(
                    "com.example.inventory",
                    "{\"gold\":100}");

            pair.Payload.participantSchemaVersion =
                0;

            pair.Inventory.participantSchemaVersion =
                0;

            AssertRejected(
                CreateBatch(
                    pair));
        }

        [Test]
        public void NonCanonicalSerializerIdIsRejected()
        {
            EntryPair pair =
                Entry(
                    "com.example.inventory",
                    "{\"gold\":100}");

            pair.Payload.serializerId =
                " EchoDevGames.Unity-Json ";

            pair.Inventory.serializerId =
                " EchoDevGames.Unity-Json ";

            AssertRejected(
                CreateBatch(
                    pair));
        }

        [Test]
        public void UnsupportedFlagsAreRejected()
        {
            EntryPair pair =
                Entry(
                    "com.example.inventory",
                    "{\"gold\":100}");

            pair.Payload.flags =
                1;

            pair.Inventory.flags =
                1;

            AssertRejected(
                CreateBatch(
                    pair));
        }

        [Test]
        public void ByteProviderReferenceIsRejected()
        {
            EntryPair pair =
                Entry(
                    "com.example.inventory",
                    "{\"gold\":100}");

            pair.Payload.byteProviderReference =
                "payload.bin";

            AssertRejected(
                CreateBatch(
                    pair));
        }

        [Test]
        public void InventoryMismatchIsRejected()
        {
            EntryPair pair =
                Entry(
                    "com.example.inventory",
                    "{\"gold\":100}");

            pair.Inventory.required =
                !pair.Inventory.required;

            AssertRejected(
                CreateBatch(
                    pair));
        }

        [Test]
        public void InlineByteLengthMismatchIsRejected()
        {
            EntryPair pair =
                Entry(
                    "com.example.inventory",
                    "{\"gold\":100}");

            pair.Payload.byteLength++;

            pair.Inventory.byteLength =
                pair.Payload.byteLength;

            AssertRejected(
                CreateBatch(
                    pair));
        }

        [Test]
        public void InlineChecksumMismatchIsRejected()
        {
            EntryPair pair =
                Entry(
                    "com.example.inventory",
                    "{\"gold\":100}");

            pair.Payload.checksum =
                new string(
                    '0',
                    64);

            pair.Inventory.checksum =
                pair.Payload.checksum;

            AssertRejected(
                CreateBatch(
                    pair));
        }

        [Test]
        public void BatchTotalByteMismatchIsRejected()
        {
            EntryPair pair =
                Entry(
                    "com.example.inventory",
                    "{\"gold\":100}");

            SaveParticipantCaptureBatchResult batch =
                SaveParticipantCaptureBatchResult
                    .Success(
                        new[]
                        {
                            pair.Payload
                        },
                        new[]
                        {
                            pair.Inventory
                        },
                        pair.Payload.byteLength + 1);

            AssertRejected(
                batch);
        }

        private void AssertRejected(
            SaveParticipantCaptureBatchResult batch)
        {
            SaveDocumentValidationResult result =
                SaveParticipantPublicationBatchValidator
                    .ValidateCaptureBatch(
                        batch,
                        integrity,
                        out SavePayloadEntry[] payload,
                        out SavePayloadInventoryEntry[] inventory);

            Assert.That(
                result.Succeeded,
                Is.False);

            Assert.That(
                payload,
                Is.Empty);

            Assert.That(
                inventory,
                Is.Empty);
        }

        private SaveParticipantCaptureBatchResult
            CreateBatch(
                params EntryPair[] pairs)
        {
            SavePayloadEntry[] payload =
                new SavePayloadEntry[
                    pairs.Length];

            SavePayloadInventoryEntry[] inventory =
                new SavePayloadInventoryEntry[
                    pairs.Length];

            long totalBytes =
                0L;

            for (int i = 0;
                 i < pairs.Length;
                 i++)
            {
                payload[i] =
                    pairs[i].Payload;

                inventory[i] =
                    pairs[i].Inventory;

                totalBytes +=
                    payload[i].byteLength;
            }

            return SaveParticipantCaptureBatchResult
                .Success(
                    payload,
                    inventory,
                    totalBytes);
        }

        private EntryPair Entry(
            string participantId,
            string serializedPayload)
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes(
                    serializedPayload);

            Assert.That(
                integrity.Calculate(
                    bytes,
                    out string checksum)
                    .Succeeded,
                Is.True);

            SavePayloadEntry payload =
                new SavePayloadEntry
                {
                    participantId =
                        participantId,
                    participantSchemaVersion =
                        3,
                    serializerId =
                        UnityJsonSaveSerializer
                            .StableId,
                    required =
                        true,
                    serializedPayload =
                        serializedPayload,
                    byteProviderReference =
                        string.Empty,
                    byteLength =
                        bytes.LongLength,
                    checksum =
                        checksum,
                    flags =
                        0
                };

            SavePayloadInventoryEntry inventory =
                new SavePayloadInventoryEntry
                {
                    participantId =
                        participantId,
                    participantSchemaVersion =
                        payload
                            .participantSchemaVersion,
                    serializerId =
                        payload.serializerId,
                    required =
                        payload.required,
                    byteLength =
                        payload.byteLength,
                    checksum =
                        payload.checksum,
                    flags =
                        payload.flags
                };

            return new EntryPair(
                payload,
                inventory);
        }

        private readonly struct EntryPair
        {
            internal EntryPair(
                SavePayloadEntry payload,
                SavePayloadInventoryEntry inventory)
            {
                Payload =
                    payload;
                Inventory =
                    inventory;
            }

            internal SavePayloadEntry Payload { get; }

            internal SavePayloadInventoryEntry Inventory
            {
                get;
            }
        }
    }
}
