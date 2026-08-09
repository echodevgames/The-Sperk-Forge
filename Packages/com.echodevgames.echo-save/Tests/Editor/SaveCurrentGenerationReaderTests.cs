
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveCurrentGenerationReaderTests
    {
        private UnityJsonSaveSerializer serializer;
        private Sha256IntegrityProvider integrity;
        private MemoryReadStorageBackend storage;
        private SaveParticipantRegistry registry;
        private SaveUnknownPayloadStore unknownStore;
        private SaveSlotId slotId;
        private SaveGenerationId generationId;

        [SetUp]
        public void SetUp()
        {
            serializer =
                new UnityJsonSaveSerializer();

            integrity =
                new Sha256IntegrityProvider();

            storage =
                new MemoryReadStorageBackend();

            registry =
                new SaveParticipantRegistry();

            unknownStore =
                new SaveUnknownPayloadStore();

            slotId =
                SaveSlotId.NewId();

            generationId =
                SaveGenerationId.NewId();
        }

        [Test]
        public void MissingHeadFailsAndPreservesPriorUnknownStore()
        {
            SeedPriorUnknown();

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCurrentGenerationReadStatus
                        .HeadUnavailable));

            AssertPriorUnknownPreserved();
        }

        [Test]
        public void MalformedHeadFailsAndPreservesPriorUnknownStore()
        {
            SeedPriorUnknown();

            SaveStorageKey.TryCreate(
                HeadPath(),
                out SaveStorageKey headKey);

            storage.Seed(
                headKey,
                Encoding.UTF8.GetBytes(
                    "{not-json"));

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCurrentGenerationReadStatus
                        .HeadInvalid));

            AssertPriorUnknownPreserved();
        }

        [Test]
        public void MissingGenerationFileFailsAndPreservesPriorUnknownStore()
        {
            SeedPriorUnknown();

            SeedHeadOnly();

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCurrentGenerationReadStatus
                        .GenerationUnavailable));

            AssertPriorUnknownPreserved();
        }

        [Test]
        public void AllKnownParticipantsProduceEmptyUnknownStore()
        {
            TrackingParticipant participant =
                Participant(
                    "com.example.inventory");

            Assert.That(
                registry.Register(
                    participant)
                    .Succeeded,
                Is.True);

            InstallGeneration(
                Entry(
                    "com.example.inventory",
                    "{\"gold\":100}"));

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.KnownParticipantCount,
                Is.EqualTo(1));

            Assert.That(
                result.UnknownParticipantCount,
                Is.Zero);

            Assert.That(
                unknownStore.Count,
                Is.Zero);

            Assert.That(
                participant.CaptureCalls,
                Is.Zero);

            Assert.That(
                participant.ApplyCalls,
                Is.Zero);
        }

        [Test]
        public void UnknownParticipantIsPreservedExactly()
        {
            string exact =
                "{ \"future\" : \"  keep formatting  \" }";

            SavePayloadEntry source =
                Entry(
                    "com.example.future",
                    exact,
                    7,
                    false);

            InstallGeneration(
                source);

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.UnknownParticipantCount,
                Is.EqualTo(1));

            SavePayloadEntry preserved =
                unknownStore.GetSnapshot()
                    .Entries[0];

            Assert.That(
                preserved.participantId,
                Is.EqualTo(
                    source.participantId));

            Assert.That(
                preserved.participantSchemaVersion,
                Is.EqualTo(
                    source.participantSchemaVersion));

            Assert.That(
                preserved.serializerId,
                Is.EqualTo(
                    source.serializerId));

            Assert.That(
                preserved.required,
                Is.EqualTo(
                    source.required));

            Assert.That(
                preserved.serializedPayload,
                Is.EqualTo(
                    exact));

            Assert.That(
                preserved.byteLength,
                Is.EqualTo(
                    source.byteLength));

            Assert.That(
                preserved.checksum,
                Is.EqualTo(
                    source.checksum));
        }

        [Test]
        public void CanonicalRegistrationRecognizesPersistedCanonicalId()
        {
            Assert.That(
                registry.Register(
                    Participant(
                        "com.example.inventory"))
                    .Succeeded,
                Is.True);

            InstallGeneration(
                Entry(
                    "com.example.inventory",
                    "{\"gold\":100}"));

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.KnownParticipantCount,
                Is.EqualTo(1));

            Assert.That(
                unknownStore.Count,
                Is.Zero);
        }

        [Test]
        public void RegisteredAliasRecognizesPriorPersistedId()
        {
            TrackingParticipant participant =
                Participant(
                    "com.example.inventory",
                    new SaveParticipantId(
                        "com.example.oldinventory"));

            Assert.That(
                registry.Register(
                    participant)
                    .Succeeded,
                Is.True);

            InstallGeneration(
                Entry(
                    "com.example.oldinventory",
                    "{\"gold\":100}"));

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.KnownParticipantCount,
                Is.EqualTo(1));

            Assert.That(
                result.UnknownParticipantCount,
                Is.Zero);

            Assert.That(
                unknownStore.Count,
                Is.Zero);
        }

        [Test]
        public void MultipleUnknownsAreStoredInCanonicalOrder()
        {
            InstallGeneration(
                Entry(
                    "com.example.alpha",
                    "{\"value\":1}"),
                Entry(
                    "com.example.middle",
                    "{\"value\":2}"),
                Entry(
                    "com.example.zeta",
                    "{\"value\":3}"));

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Succeeded,
                Is.True);

            SaveUnknownPayloadSnapshot snapshot =
                unknownStore.GetSnapshot();

            Assert.That(
                snapshot.Entries[0]
                    .participantId,
                Is.EqualTo(
                    "com.example.alpha"));

            Assert.That(
                snapshot.Entries[1]
                    .participantId,
                Is.EqualTo(
                    "com.example.middle"));

            Assert.That(
                snapshot.Entries[2]
                    .participantId,
                Is.EqualTo(
                    "com.example.zeta"));
        }

        [Test]
        public void CorruptWholePayloadChecksumFailsAndPreservesPriorStore()
        {
            SeedPriorUnknown();

            InstalledGeneration installed =
                InstallGeneration(
                    Entry(
                        "com.example.future",
                        "{\"value\":1}"));

            installed.Manifest.payloadChecksum =
                new string(
                    '0',
                    64);

            storage.Seed(
                installed.Keys.GenerationManifest,
                Serialize(
                    installed.Manifest));

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCurrentGenerationReadStatus
                        .GenerationInvalid));

            AssertPriorUnknownPreserved();
        }

        [Test]
        public void CorruptPerEntryChecksumFailsAndPreservesPriorStore()
        {
            SeedPriorUnknown();

            InstalledGeneration installed =
                InstallGeneration(
                    Entry(
                        "com.example.future",
                        "{\"value\":1}"));

            installed.Payload.entries[0]
                .checksum =
                new string(
                    '0',
                    64);

            installed.Manifest.payloadEntries[0]
                .checksum =
                installed.Payload.entries[0]
                    .checksum;

            byte[] mutatedPayloadBytes =
                Serialize(
                    installed.Payload);

            Assert.That(
                integrity.Calculate(
                    mutatedPayloadBytes,
                    out string wholeChecksum)
                    .Succeeded,
                Is.True);

            installed.Manifest.payloadByteLength =
                mutatedPayloadBytes.LongLength;

            installed.Manifest.payloadChecksum =
                wholeChecksum;

            storage.Seed(
                installed.Keys.GenerationPayload,
                mutatedPayloadBytes);

            storage.Seed(
                installed.Keys.GenerationManifest,
                Serialize(
                    installed.Manifest));

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCurrentGenerationReadStatus
                        .GenerationInvalid));

            AssertPriorUnknownPreserved();
        }

        [Test]
        public void PayloadInventoryMismatchFailsAndPreservesPriorStore()
        {
            SeedPriorUnknown();

            InstalledGeneration installed =
                InstallGeneration(
                    Entry(
                        "com.example.future",
                        "{\"value\":1}"));

            installed.Manifest.payloadEntries[0]
                .required =
                !installed.Manifest.payloadEntries[0]
                    .required;

            storage.Seed(
                installed.Keys.GenerationManifest,
                Serialize(
                    installed.Manifest));

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCurrentGenerationReadStatus
                        .GenerationInvalid));

            AssertPriorUnknownPreserved();
        }

        [Test]
        public void DuplicateParticipantIdFailsAndPreservesPriorStore()
        {
            SeedPriorUnknown();

            SavePayloadEntry first =
                Entry(
                    "com.example.future",
                    "{\"value\":1}");

            SavePayloadEntry duplicate =
                Entry(
                    "com.example.future",
                    "{\"value\":2}");

            InstallGeneration(
                first,
                duplicate);

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCurrentGenerationReadStatus
                        .GenerationInvalid));

            AssertPriorUnknownPreserved();
        }

        [Test]
        public void UnknownEntryCountLimitFailsAndPreservesPriorStore()
        {
            unknownStore =
                new SaveUnknownPayloadStore(
                    1,
                    1024);

            SeedPriorUnknown();

            InstallGeneration(
                Entry(
                    "com.example.alpha",
                    "{\"value\":1}"),
                Entry(
                    "com.example.beta",
                    "{\"value\":2}"));

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCurrentGenerationReadStatus
                        .UnknownPayloadRejected));

            AssertPriorUnknownPreserved();
        }

        [Test]
        public void ReaderPerformsNoStorageMutation()
        {
            InstallGeneration(
                Entry(
                    "com.example.future",
                    "{\"value\":1}"));

            storage.ResetMutationCount();

            SaveCurrentGenerationReadResult result =
                CreateReader()
                    .ReadCurrent(
                        slotId);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                storage.MutationCalls,
                Is.Zero);
        }

        private SaveCurrentGenerationReader
            CreateReader() =>
            new SaveCurrentGenerationReader(
                storage,
                serializer,
                integrity,
                registry,
                unknownStore);

        private TrackingParticipant Participant(
            string id,
            params SaveParticipantId[] aliases) =>
            new TrackingParticipant(
                new SaveParticipantDescriptor(
                    new SaveParticipantId(
                        id),
                    1,
                    SaveParticipantCriticality.Required,
                    SaveMissingPayloadPolicy
                        .InitializeDefault,
                    default,
                    aliases));

        private void SeedPriorUnknown()
        {
            Assert.That(
                unknownStore.TryReplace(
                    new[]
                    {
                        Entry(
                            "com.example.prior",
                            "{\"prior\":true}")
                    })
                    .Succeeded,
                Is.True);
        }

        private void AssertPriorUnknownPreserved()
        {
            SaveUnknownPayloadSnapshot snapshot =
                unknownStore.GetSnapshot();

            Assert.That(
                snapshot.Count,
                Is.EqualTo(1));

            Assert.That(
                snapshot.Entries[0]
                    .participantId,
                Is.EqualTo(
                    "com.example.prior"));

            Assert.That(
                snapshot.Entries[0]
                    .serializedPayload,
                Is.EqualTo(
                    "{\"prior\":true}"));
        }

        private void SeedHeadOnly()
        {
            SaveHeadPointer head =
                new SaveHeadPointer
                {
                    slotId =
                        slotId.Value,
                    currentGenerationId =
                        generationId.Value,
                    previousGenerationId =
                        string.Empty,
                    updateSequence =
                        1
                };

            SaveStorageKey.TryCreate(
                HeadPath(),
                out SaveStorageKey headKey);

            storage.Seed(
                headKey,
                Serialize(
                    head));
        }

        private InstalledGeneration InstallGeneration(
            params SavePayloadEntry[] entries)
        {
            SaveGenerationStorageKeys.TryCreate(
                slotId,
                generationId,
                out SaveGenerationStorageKeys keys);

            SavePayloadInventoryEntry[] inventory =
                new SavePayloadInventoryEntry[
                    entries.Length];

            for (int i = 0;
                 i < entries.Length;
                 i++)
            {
                inventory[i] =
                    Inventory(
                        entries[i]);
            }

            SavePayloadDocument payload =
                new SavePayloadDocument
                {
                    slotId =
                        slotId.Value,
                    generationId =
                        generationId.Value,
                    entries =
                        entries
                };

            byte[] payloadBytes =
                Serialize(
                    payload);

            Assert.That(
                integrity.Calculate(
                    payloadBytes,
                    out string payloadChecksum)
                    .Succeeded,
                Is.True);

            SaveManifest manifest =
                new SaveManifest
                {
                    slotId =
                        slotId.Value,
                    generationId =
                        generationId.Value,
                    createdUtc =
                        "2026-08-09T20:00:00.0000000Z",
                    updatedUtc =
                        "2026-08-09T20:00:00.0000000Z",
                    saveKind =
                        "participant",
                    projectId =
                        "com.example.game",
                    projectVersion =
                        "1.0.0",
                    buildId =
                        "test",
                    displayName =
                        "Reader Test",
                    payloadByteLength =
                        payloadBytes.LongLength,
                    payloadChecksum =
                        payloadChecksum,
                    integrityAlgorithm =
                        integrity.Id.Value,
                    payloadEntries =
                        inventory,
                    commitState =
                        SaveGenerationCommitState
                            .Committed
                };

            SaveHeadPointer head =
                new SaveHeadPointer
                {
                    slotId =
                        slotId.Value,
                    currentGenerationId =
                        generationId.Value,
                    previousGenerationId =
                        string.Empty,
                    updateSequence =
                        1
                };

            storage.Seed(
                keys.GenerationPayload,
                payloadBytes);

            storage.Seed(
                keys.GenerationManifest,
                Serialize(
                    manifest));

            storage.Seed(
                keys.Head,
                Serialize(
                    head));

            return new InstalledGeneration(
                keys,
                payload,
                manifest,
                head);
        }

        private SavePayloadEntry Entry(
            string participantId,
            string serializedPayload,
            int schemaVersion = 1,
            bool required = true)
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

            return new SavePayloadEntry
            {
                participantId =
                    participantId,
                participantSchemaVersion =
                    schemaVersion,
                serializerId =
                    UnityJsonSaveSerializer
                        .StableId,
                required =
                    required,
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
        }

        private static SavePayloadInventoryEntry Inventory(
            SavePayloadEntry entry) =>
            new SavePayloadInventoryEntry
            {
                participantId =
                    entry.participantId,
                participantSchemaVersion =
                    entry.participantSchemaVersion,
                serializerId =
                    entry.serializerId,
                required =
                    entry.required,
                byteLength =
                    entry.byteLength,
                checksum =
                    entry.checksum,
                flags =
                    entry.flags
            };

        private byte[] Serialize<T>(
            T value)
        {
            Assert.That(
                serializer.Serialize(
                    value,
                    out string json)
                    .Succeeded,
                Is.True);

            return Encoding.UTF8.GetBytes(
                json);
        }

        private string HeadPath() =>
            "slots/" +
            slotId.Value +
            "/head.json";

        private sealed class InstalledGeneration
        {
            internal InstalledGeneration(
                SaveGenerationStorageKeys keys,
                SavePayloadDocument payload,
                SaveManifest manifest,
                SaveHeadPointer head)
            {
                Keys =
                    keys;

                Payload =
                    payload;

                Manifest =
                    manifest;

                Head =
                    head;
            }

            internal SaveGenerationStorageKeys Keys { get; }

            internal SavePayloadDocument Payload { get; }

            internal SaveManifest Manifest { get; }

            internal SaveHeadPointer Head { get; }
        }

        private sealed class TrackingParticipant :
            ISaveParticipant
        {
            internal TrackingParticipant(
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

                return SaveParticipantCaptureResult
                    .Failure(
                        "Reader tests must not capture participants.");
            }

            public SaveParticipantApplyResult Apply(
                object detachedState)
            {
                ApplyCalls++;

                return SaveParticipantApplyResult
                    .Failure(
                        "Reader tests must not apply participants.");
            }
        }

        private sealed class MemoryReadStorageBackend :
            ISaveStorageBackend
        {
            private readonly
                Dictionary<string, byte[]> data =
                    new Dictionary<string, byte[]>(
                        StringComparer.Ordinal);

            internal int MutationCalls { get; private set; }

            public SaveStorageBackendId Id =>
                new SaveStorageBackendId(
                    "tests.read-memory");

            public string RootPath =>
                "memory://chronicle";

            public SaveStorageResult Initialize() =>
                SaveStorageResult.Success(
                    "Memory read backend initialized.");

            public SaveStorageResult Exists(
                SaveStorageKey key,
                out bool exists)
            {
                exists =
                    data.ContainsKey(
                        key.Value);

                return SaveStorageResult.Success(
                    "Memory read backend existence check completed.");
            }

            public SaveStorageReadResult Read(
                SaveStorageKey key)
            {
                if (!data.TryGetValue(
                        key.Value,
                        out byte[] value))
                {
                    return new SaveStorageReadResult(
                        new SaveStorageResult(
                            SaveStorageStatus.NotFound,
                            EchoSaveDiagnosticCodes
                                .StorageNotFound,
                            "Memory read backend entry not found."),
                        Array.Empty<byte>());
                }

                return new SaveStorageReadResult(
                    SaveStorageResult.Success(
                        "Memory read backend entry read."),
                    value);
            }

            public SaveStorageResult WriteNew(
                SaveStorageKey key,
                byte[] value)
            {
                MutationCalls++;

                return new SaveStorageResult(
                    SaveStorageStatus.Failed,
                    "ESV-TEST-MUTATION",
                    "Reader tests forbid storage mutation.");
            }

            public SaveStorageResult Delete(
                SaveStorageKey key)
            {
                MutationCalls++;

                return new SaveStorageResult(
                    SaveStorageStatus.Failed,
                    "ESV-TEST-MUTATION",
                    "Reader tests forbid storage mutation.");
            }

            public SaveStorageResult Shutdown() =>
                SaveStorageResult.Success(
                    "Memory read backend shut down.");

            internal void Seed(
                SaveStorageKey key,
                byte[] value)
            {
                data[
                    key.Value] =
                    value == null
                        ? Array.Empty<byte>()
                        : (byte[])value.Clone();
            }

            internal void ResetMutationCount()
            {
                MutationCalls =
                    0;
            }
        }
    }
}
