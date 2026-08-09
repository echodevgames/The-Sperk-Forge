
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantGenerationPublicationTests
    {
        private string sandboxParent;
        private string backendRoot;
        private LocalFileSaveStorageBackend local;
        private FaultingBackend backend;
        private UnityJsonSaveSerializer serializer;
        private Sha256IntegrityProvider integrity;
        private FixedClock clock;
        private SaveSlotId slotId;

        [SetUp]
        public void SetUp()
        {
            sandboxParent =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-M3-03-Publish-" +
                    Guid.NewGuid()
                        .ToString("N"));

            backendRoot =
                Path.Combine(
                    sandboxParent,
                    "Chronicle");

            local =
                new LocalFileSaveStorageBackend(
                    backendRoot);

            Assert.That(
                local.Initialize()
                    .Succeeded,
                Is.True);

            backend =
                new FaultingBackend(
                    local);

            serializer =
                new UnityJsonSaveSerializer();

            integrity =
                new Sha256IntegrityProvider();

            clock =
                new FixedClock(
                    new DateTime(
                        2026,
                        8,
                        9,
                        19,
                        30,
                        0,
                        DateTimeKind.Utc));

            slotId =
                SaveSlotId.NewId();
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(
                    sandboxParent))
            {
                Directory.Delete(
                    sandboxParent,
                    true);
            }
        }

        [Test]
        public void FirstParticipantPublicationWritesGenerationBeforeHead()
        {
            SaveGenerationPublicationResult result =
                CreateCoordinator()
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-a",
                        "First",
                        Batch(
                            Pair(
                                "com.example.inventory",
                                "{\"gold\":100}")));

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.GenerationPublished,
                Is.True);

            Assert.That(
                result.HeadPublished,
                Is.True);

            SaveGenerationStorageKeys.TryCreate(
                slotId,
                result.GenerationId,
                out SaveGenerationStorageKeys keys);

            SavePayloadDocument payload =
                ReadPayload(
                    keys.GenerationPayload);

            SaveManifest manifest =
                ReadManifest(
                    keys.GenerationManifest);

            Assert.That(
                payload.entries.Length,
                Is.EqualTo(1));

            Assert.That(
                manifest.payloadEntries.Length,
                Is.EqualTo(1));

            Assert.That(
                manifest.saveKind,
                Is.EqualTo(
                    "participant"));

            SaveHeadPointer head =
                ReadHead(
                    keys.Head);

            Assert.That(
                head.currentGenerationId,
                Is.EqualTo(
                    result.GenerationId.Value));

            Assert.That(
                head.previousGenerationId,
                Is.Empty);
        }

        [Test]
        public void MultiParticipantOrderAndInventorySurviveDiskRoundTrip()
        {
            SaveGenerationPublicationResult result =
                CreateCoordinator()
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-a",
                        "Multi",
                        Batch(
                            Pair(
                                "com.example.inventory",
                                "{\"gold\":100}"),
                            Pair(
                                "com.example.quests",
                                "{\"quest\":7}"),
                            Pair(
                                "com.example.settings",
                                "{\"volume\":5}")));

            Assert.That(
                result.Succeeded,
                Is.True);

            SaveGenerationStorageKeys.TryCreate(
                slotId,
                result.GenerationId,
                out SaveGenerationStorageKeys keys);

            SavePayloadDocument payload =
                ReadPayload(
                    keys.GenerationPayload);

            SaveManifest manifest =
                ReadManifest(
                    keys.GenerationManifest);

            Assert.That(
                payload.entries[0]
                    .participantId,
                Is.EqualTo(
                    "com.example.inventory"));

            Assert.That(
                payload.entries[1]
                    .participantId,
                Is.EqualTo(
                    "com.example.quests"));

            Assert.That(
                payload.entries[2]
                    .participantId,
                Is.EqualTo(
                    "com.example.settings"));

            for (int i = 0;
                 i < payload.entries.Length;
                 i++)
            {
                Assert.That(
                    manifest.payloadEntries[i]
                        .participantId,
                    Is.EqualTo(
                        payload.entries[i]
                            .participantId));

                Assert.That(
                    manifest.payloadEntries[i]
                        .checksum,
                    Is.EqualTo(
                        payload.entries[i]
                            .checksum));

                Assert.That(
                    manifest.payloadEntries[i]
                        .byteLength,
                    Is.EqualTo(
                        payload.entries[i]
                            .byteLength));
            }
        }

        [Test]
        public void SecondParticipantPublicationAdvancesHeadAndPreservesFirst()
        {
            SaveGenerationPublicationCoordinator coordinator =
                CreateCoordinator();

            SaveGenerationPublicationResult first =
                coordinator
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-a",
                        "First",
                        Batch(
                            Pair(
                                "com.example.inventory",
                                "{\"gold\":100}")));

            clock.Utc =
                clock.Utc.AddMinutes(
                    1);

            SaveGenerationPublicationResult second =
                coordinator
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-b",
                        "Second",
                        Batch(
                            Pair(
                                "com.example.inventory",
                                "{\"gold\":150}")));

            Assert.That(
                first.Succeeded,
                Is.True);

            Assert.That(
                second.Succeeded,
                Is.True);

            SaveGenerationStorageKeys.TryCreate(
                slotId,
                second.GenerationId,
                out SaveGenerationStorageKeys secondKeys);

            SaveHeadPointer head =
                ReadHead(
                    secondKeys.Head);

            Assert.That(
                head.currentGenerationId,
                Is.EqualTo(
                    second.GenerationId.Value));

            Assert.That(
                head.previousGenerationId,
                Is.EqualTo(
                    first.GenerationId.Value));

            Assert.That(
                head.updateSequence,
                Is.EqualTo(2));

            SaveGenerationStorageKeys.TryCreate(
                slotId,
                first.GenerationId,
                out SaveGenerationStorageKeys firstKeys);

            Assert.That(
                local.Read(
                    firstKeys.GenerationPayload)
                    .Succeeded,
                Is.True);

            Assert.That(
                local.Read(
                    firstKeys.GenerationManifest)
                    .Succeeded,
                Is.True);
        }

        [Test]
        public void InvalidBatchProducesZeroStorageMutation()
        {
            EntryPair pair =
                Pair(
                    "com.example.inventory",
                    "{\"gold\":100}");

            pair.Payload.byteLength++;

            pair.Inventory.byteLength =
                pair.Payload.byteLength;

            SaveGenerationPublicationResult result =
                CreateCoordinator()
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-a",
                        "Invalid",
                        Batch(
                            pair));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveGenerationPublicationStatus
                        .InvalidRequest));

            Assert.That(
                backend.MutationCalls,
                Is.Zero);
        }

        [TestCase(FaultPoint.CandidatePayloadWrite)]
        [TestCase(FaultPoint.CandidateManifestWrite)]
        [TestCase(FaultPoint.CandidatePayloadReadCorruption)]
        [TestCase(FaultPoint.GenerationPublication)]
        [TestCase(FaultPoint.PublishedPayloadReadCorruption)]
        public void PreHeadParticipantPublicationFailurePreservesOldHead(
            FaultPoint fault)
        {
            SaveGenerationPublicationCoordinator coordinator =
                CreateCoordinator();

            SaveGenerationPublicationResult first =
                coordinator
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-a",
                        "First",
                        Batch(
                            Pair(
                                "com.example.inventory",
                                "{\"gold\":100}")));

            Assert.That(
                first.Succeeded,
                Is.True);

            byte[] oldHead =
                ReadCurrentHeadBytes();

            backend.Fault =
                fault;

            SaveGenerationPublicationResult second =
                coordinator
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-b",
                        "Second",
                        Batch(
                            Pair(
                                "com.example.inventory",
                                "{\"gold\":200}")));

            Assert.That(
                second.Succeeded,
                Is.False);

            Assert.That(
                second.HeadPublished,
                Is.False);

            Assert.That(
                ReadCurrentHeadBytes(),
                Is.EqualTo(
                    oldHead));

            Assert.That(
                ReadCurrentHead()
                    .currentGenerationId,
                Is.EqualTo(
                    first.GenerationId.Value));
        }

        [Test]
        public void HeadSerializationFailureLeavesParticipantGenerationOrphaned()
        {
            SaveGenerationPublicationResult first =
                CreateCoordinator()
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-a",
                        "First",
                        Batch(
                            Pair(
                                "com.example.inventory",
                                "{\"gold\":100}")));

            byte[] oldHead =
                ReadCurrentHeadBytes();

            SaveGenerationPublicationCoordinator coordinator =
                new SaveGenerationPublicationCoordinator(
                    backend,
                    new FaultingHeadSerializer(
                        serializer),
                    integrity,
                    clock,
                    SaveGenerationId.NewId);

            SaveGenerationPublicationResult second =
                coordinator
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-b",
                        "Second",
                        Batch(
                            Pair(
                                "com.example.inventory",
                                "{\"gold\":200}")));

            Assert.That(
                second.Status,
                Is.EqualTo(
                    SaveGenerationPublicationStatus
                        .HeadPublicationFailed));

            Assert.That(
                second.GenerationPublished,
                Is.True);

            Assert.That(
                second.HeadPublished,
                Is.False);

            Assert.That(
                ReadCurrentHeadBytes(),
                Is.EqualTo(
                    oldHead));

            Assert.That(
                ReadCurrentHead()
                    .currentGenerationId,
                Is.EqualTo(
                    first.GenerationId.Value));
        }

        [Test]
        public void HeadPublicationFailureLeavesParticipantGenerationOrphaned()
        {
            SaveGenerationPublicationResult first =
                CreateCoordinator()
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-a",
                        "First",
                        Batch(
                            Pair(
                                "com.example.inventory",
                                "{\"gold\":100}")));

            byte[] oldHead =
                ReadCurrentHeadBytes();

            backend.Fault =
                FaultPoint.HeadPublication;

            SaveGenerationPublicationResult second =
                CreateCoordinator()
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-b",
                        "Second",
                        Batch(
                            Pair(
                                "com.example.inventory",
                                "{\"gold\":200}")));

            Assert.That(
                second.Status,
                Is.EqualTo(
                    SaveGenerationPublicationStatus
                        .HeadPublicationFailed));

            Assert.That(
                second.GenerationPublished,
                Is.True);

            Assert.That(
                second.HeadPublished,
                Is.False);

            Assert.That(
                ReadCurrentHeadBytes(),
                Is.EqualTo(
                    oldHead));

            SaveGenerationStorageKeys.TryCreate(
                slotId,
                second.GenerationId,
                out SaveGenerationStorageKeys secondKeys);

            Assert.That(
                local.Read(
                    secondKeys.GenerationManifest)
                    .Succeeded,
                Is.True);

            Assert.That(
                ReadCurrentHead()
                    .currentGenerationId,
                Is.EqualTo(
                    first.GenerationId.Value));
        }

        [Test]
        public void ExistingEmptyTransportPublicationPathStillWorks()
        {
            SaveGenerationPublicationResult result =
                CreateCoordinator()
                    .PublishEmptyTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-a",
                        "Empty");

            Assert.That(
                result.Succeeded,
                Is.True);

            SaveGenerationStorageKeys.TryCreate(
                slotId,
                result.GenerationId,
                out SaveGenerationStorageKeys keys);

            SavePayloadDocument payload =
                ReadPayload(
                    keys.GenerationPayload);

            Assert.That(
                payload.entries,
                Is.Empty);
        }

        private SaveGenerationPublicationCoordinator
            CreateCoordinator() =>
            new SaveGenerationPublicationCoordinator(
                backend,
                serializer,
                integrity,
                clock,
                SaveGenerationId.NewId);

        private SaveParticipantCaptureBatchResult
            Batch(
                params EntryPair[] pairs)
        {
            SavePayloadEntry[] payload =
                new SavePayloadEntry[
                    pairs.Length];

            SavePayloadInventoryEntry[] inventory =
                new SavePayloadInventoryEntry[
                    pairs.Length];

            long total =
                0L;

            for (int i = 0;
                 i < pairs.Length;
                 i++)
            {
                payload[i] =
                    pairs[i].Payload;

                inventory[i] =
                    pairs[i].Inventory;

                total +=
                    payload[i].byteLength;
            }

            return SaveParticipantCaptureBatchResult
                .Success(
                    payload,
                    inventory,
                    total);
        }

        private EntryPair Pair(
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

        private SavePayloadDocument ReadPayload(
            SaveStorageKey key)
        {
            SaveStorageReadResult read =
                local.Read(
                    key);

            Assert.That(
                read.Succeeded,
                Is.True);

            Assert.That(
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SavePayloadDocument payload)
                    .Succeeded,
                Is.True);

            return payload;
        }

        private SaveManifest ReadManifest(
            SaveStorageKey key)
        {
            SaveStorageReadResult read =
                local.Read(
                    key);

            Assert.That(
                read.Succeeded,
                Is.True);

            Assert.That(
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SaveManifest manifest)
                    .Succeeded,
                Is.True);

            return manifest;
        }

        private SaveHeadPointer ReadCurrentHead()
        {
            SaveStorageKey.TryCreate(
                "slots/" +
                slotId.Value +
                "/head.json",
                out SaveStorageKey headKey);

            return ReadHead(
                headKey);
        }

        private byte[] ReadCurrentHeadBytes()
        {
            SaveStorageKey.TryCreate(
                "slots/" +
                slotId.Value +
                "/head.json",
                out SaveStorageKey headKey);

            SaveStorageReadResult read =
                local.Read(
                    headKey);

            Assert.That(
                read.Succeeded,
                Is.True);

            return read.Data;
        }

        private SaveHeadPointer ReadHead(
            SaveStorageKey key)
        {
            SaveStorageReadResult read =
                local.Read(
                    key);

            Assert.That(
                read.Succeeded,
                Is.True);

            Assert.That(
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SaveHeadPointer head)
                    .Succeeded,
                Is.True);

            return head;
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

        public enum FaultPoint
        {
            None = 0,
            CandidatePayloadWrite = 1,
            CandidateManifestWrite = 2,
            CandidatePayloadReadCorruption = 3,
            GenerationPublication = 4,
            PublishedPayloadReadCorruption = 5,
            HeadPublication = 6
        }

        private sealed class FaultingBackend :
            ISaveStoragePublicationBackend
        {
            private readonly
                LocalFileSaveStorageBackend inner;

            internal FaultingBackend(
                LocalFileSaveStorageBackend inner)
            {
                this.inner =
                    inner;
            }

            internal FaultPoint Fault { get; set; }

            internal int MutationCalls { get; private set; }

            public SaveStorageBackendId Id =>
                inner.Id;

            public string RootPath =>
                inner.RootPath;

            public SaveStoragePublicationCapabilities
                PublicationCapabilities =>
                inner.PublicationCapabilities;

            public SaveStorageResult Initialize() =>
                inner.Initialize();

            public SaveStorageResult Exists(
                SaveStorageKey key,
                out bool exists) =>
                inner.Exists(
                    key,
                    out exists);

            public SaveStorageReadResult Read(
                SaveStorageKey key)
            {
                SaveStorageReadResult result =
                    inner.Read(
                        key);

                if (!result.Succeeded)
                {
                    return result;
                }

                bool corruptCandidate =
                    Fault ==
                        FaultPoint
                            .CandidatePayloadReadCorruption &&
                    key.Value.Contains(
                        "/incomplete/") &&
                    key.Value.EndsWith(
                        "/payload.json",
                        StringComparison.Ordinal);

                bool corruptPublished =
                    Fault ==
                        FaultPoint
                            .PublishedPayloadReadCorruption &&
                    key.Value.Contains(
                        "/generations/") &&
                    key.Value.EndsWith(
                        "/payload.json",
                        StringComparison.Ordinal);

                if (!corruptCandidate &&
                    !corruptPublished)
                {
                    return result;
                }

                byte[] corrupted =
                    result.Data;

                if (corrupted.Length > 0)
                {
                    corrupted[
                        corrupted.Length - 1] =
                        (byte)'!';
                }

                return new SaveStorageReadResult(
                    SaveStorageResult.Success(
                        "Fault-injected corrupted payload read."),
                    corrupted);
            }

            public SaveStorageResult WriteNew(
                SaveStorageKey key,
                byte[] data)
            {
                MutationCalls++;

                if (Fault ==
                        FaultPoint
                            .CandidatePayloadWrite &&
                    key.Value.Contains(
                        "/incomplete/") &&
                    key.Value.EndsWith(
                        "/payload.json",
                        StringComparison.Ordinal))
                {
                    return InjectedFailure(
                        "Candidate payload write fault.");
                }

                if (Fault ==
                        FaultPoint
                            .CandidateManifestWrite &&
                    key.Value.Contains(
                        "/incomplete/") &&
                    key.Value.EndsWith(
                        "/manifest.json",
                        StringComparison.Ordinal))
                {
                    return InjectedFailure(
                        "Candidate manifest write fault.");
                }

                return inner.WriteNew(
                    key,
                    data);
            }

            public SaveStorageResult Delete(
                SaveStorageKey key)
            {
                MutationCalls++;

                return inner.Delete(
                    key);
            }

            public SaveStorageResult PublishNewTree(
                SaveStorageKey sourceDirectoryKey,
                SaveStorageKey destinationDirectoryKey)
            {
                MutationCalls++;

                if (Fault ==
                    FaultPoint.GenerationPublication)
                {
                    return InjectedFailure(
                        "Generation publication fault.");
                }

                return inner.PublishNewTree(
                    sourceDirectoryKey,
                    destinationDirectoryKey);
            }

            public SaveStorageResult PublishCurrentObject(
                SaveStorageKey key,
                byte[] data)
            {
                MutationCalls++;

                if (Fault ==
                    FaultPoint.HeadPublication)
                {
                    return InjectedFailure(
                        "Head publication fault.");
                }

                return inner.PublishCurrentObject(
                    key,
                    data);
            }

            public SaveStorageResult Shutdown() =>
                inner.Shutdown();

            private static SaveStorageResult
                InjectedFailure(
                    string message) =>
            new SaveStorageResult(
                SaveStorageStatus.Failed,
                "ESV-TEST-FAULT",
                message);
        }

        private sealed class FaultingHeadSerializer :
            ISaveSerializer
        {
            private readonly ISaveSerializer inner;

            internal FaultingHeadSerializer(
                ISaveSerializer inner)
            {
                this.inner =
                    inner;
            }

            public SaveSerializerId Id =>
                inner.Id;

            public SaveSerializerResult Serialize<T>(
                T value,
                out string serialized)
            {
                if (value is SaveHeadPointer)
                {
                    serialized =
                        string.Empty;

                    return new SaveSerializerResult(
                        SaveSerializerStatus.Failed,
                        "ESV-TEST-HEAD-SERIALIZE",
                        "Fault-injected head serialization failure.");
                }

                return inner.Serialize(
                    value,
                    out serialized);
            }

            public SaveSerializerResult Deserialize<T>(
                string serialized,
                out T value) =>
                inner.Deserialize(
                    serialized,
                    out value);
        }

        private sealed class FixedClock :
            ISaveClock
        {
            internal FixedClock(
                DateTime utc)
            {
                Utc =
                    utc;
            }

            internal DateTime Utc { get; set; }

            public DateTime UtcNow =>
                Utc;

            public double MonotonicSeconds =>
                0d;
        }
    }
}
