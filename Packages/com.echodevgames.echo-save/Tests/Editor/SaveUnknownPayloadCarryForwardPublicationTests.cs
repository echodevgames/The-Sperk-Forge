
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveUnknownPayloadCarryForwardPublicationTests
    {
        private string sandboxParent;
        private LocalFileSaveStorageBackend local;
        private TrackingPublicationBackend backend;
        private UnityJsonSaveSerializer serializer;
        private Sha256IntegrityProvider integrity;
        private FixedClock clock;
        private SaveParticipantRegistry registry;
        private SaveUnknownPayloadStore unknownStore;
        private SaveGenerationPublicationCoordinator publication;
        private SaveUnknownPayloadCarryForwardCoordinator carryForward;
        private SaveSlotId slotId;

        [SetUp]
        public void SetUp()
        {
            sandboxParent =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-M3-05-Carry-" +
                    Guid.NewGuid()
                        .ToString("N"));

            local =
                new LocalFileSaveStorageBackend(
                    Path.Combine(
                        sandboxParent,
                        "Chronicle"));

            Assert.That(
                local.Initialize()
                    .Succeeded,
                Is.True);

            backend =
                new TrackingPublicationBackend(
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
                        20,
                        30,
                        0,
                        DateTimeKind.Utc));

            registry =
                new SaveParticipantRegistry();

            unknownStore =
                new SaveUnknownPayloadStore();

            slotId =
                SaveSlotId.NewId();

            RebuildCoordinators();
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
        public void FreshKnownAndOpaqueUnknownPublishAsNextGeneration()
        {
            SourceState source =
                InstallSource();

            Register(
                Participant(
                    "com.example.inventory"));

            string exactUnknown =
                source.Unknown.Payload
                    .serializedPayload;

            SaveCarryForwardPublicationResult result =
                PublishCarryForward(
                    source.Snapshot,
                    "{\"gold\":150}");

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.FreshParticipantCount,
                Is.EqualTo(1));

            Assert.That(
                result.PreservedUnknownCount,
                Is.EqualTo(1));

            SaveGenerationStorageKeys.TryCreate(
                slotId,
                result.PublishedGenerationId,
                out SaveGenerationStorageKeys newKeys);

            SavePayloadDocument payload =
                ReadPayload(
                    newKeys.GenerationPayload);

            Assert.That(
                payload.entries.Length,
                Is.EqualTo(2));

            Assert.That(
                payload.entries[0]
                    .participantId,
                Is.EqualTo(
                    "com.example.future"));

            Assert.That(
                payload.entries[0]
                    .serializedPayload,
                Is.EqualTo(
                    exactUnknown));

            Assert.That(
                payload.entries[0]
                    .byteLength,
                Is.EqualTo(
                    source.Unknown.Payload
                        .byteLength));

            Assert.That(
                payload.entries[0]
                    .checksum,
                Is.EqualTo(
                    source.Unknown.Payload
                        .checksum));

            Assert.That(
                payload.entries[1]
                    .participantId,
                Is.EqualTo(
                    "com.example.inventory"));

            SaveHeadPointer head =
                ReadCurrentHead();

            Assert.That(
                head.currentGenerationId,
                Is.EqualTo(
                    result.PublishedGenerationId
                        .Value));

            Assert.That(
                head.previousGenerationId,
                Is.EqualTo(
                    source.GenerationId.Value));

            SaveGenerationStorageKeys.TryCreate(
                slotId,
                source.GenerationId,
                out SaveGenerationStorageKeys sourceKeys);

            Assert.That(
                local.Read(
                    sourceKeys.GenerationPayload)
                    .Succeeded,
                Is.True);

            Assert.That(
                source.Snapshot.SourceGenerationId,
                Is.EqualTo(
                    source.GenerationId));
        }

        [Test]
        public void SuccessfulCarryForwardMakesOldSnapshotStale()
        {
            SourceState source =
                InstallSource();

            Register(
                Participant(
                    "com.example.inventory"));

            SaveCarryForwardPublicationResult first =
                PublishCarryForward(
                    source.Snapshot,
                    "{\"gold\":150}");

            Assert.That(
                first.Succeeded,
                Is.True);

            backend.ResetMutationCount();

            SaveCarryForwardPublicationResult second =
                PublishCarryForward(
                    source.Snapshot,
                    "{\"gold\":175}");

            Assert.That(
                second.Status,
                Is.EqualTo(
                    SaveCarryForwardPublicationStatus
                        .StaleSource));

            Assert.That(
                backend.MutationCalls,
                Is.Zero);
        }

        [Test]
        public void SnapshotWithoutProvenanceFailsBeforeStorageMutation()
        {
            SourceState source =
                InstallSource();

            Register(
                Participant(
                    "com.example.inventory"));

            SaveUnknownPayloadSnapshot noProvenance =
                new SaveUnknownPayloadSnapshot(
                    new[]
                    {
                        source.Unknown.Payload
                    },
                    source.Unknown.Payload
                        .byteLength);

            backend.ResetMutationCount();

            SaveCarryForwardPublicationResult result =
                PublishCarryForward(
                    noProvenance,
                    "{\"gold\":150}");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCarryForwardPublicationStatus
                        .MissingProvenance));

            Assert.That(
                backend.MutationCalls,
                Is.Zero);
        }

        [Test]
        public void SnapshotSlotMismatchFailsBeforeStorageMutation()
        {
            SourceState source =
                InstallSource();

            Register(
                Participant(
                    "com.example.inventory"));

            SaveUnknownPayloadSnapshot wrongSlot =
                new SaveUnknownPayloadSnapshot(
                    SaveUnknownPayloadSnapshot
                        .CloneEntries(
                            source.Snapshot.Entries),
                    source.Snapshot.TotalPayloadBytes,
                    SaveSlotId.NewId(),
                    source.GenerationId,
                    true);

            backend.ResetMutationCount();

            SaveCarryForwardPublicationResult result =
                PublishCarryForward(
                    wrongSlot,
                    "{\"gold\":150}");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCarryForwardPublicationStatus
                        .SlotMismatch));

            Assert.That(
                backend.MutationCalls,
                Is.Zero);
        }

        [Test]
        public void HeadAdvancedAfterSnapshotFailsFreshnessWithZeroMutation()
        {
            SourceState source =
                InstallSource();

            Register(
                Participant(
                    "com.example.inventory"));

            SaveGenerationPublicationResult intervening =
                publication
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-intervening",
                        "Intervening",
                        Batch(
                            Pair(
                                "com.example.inventory",
                                "{\"gold\":125}")));

            Assert.That(
                intervening.Succeeded,
                Is.True);

            backend.ResetMutationCount();

            SaveCarryForwardPublicationResult result =
                PublishCarryForward(
                    source.Snapshot,
                    "{\"gold\":150}");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCarryForwardPublicationStatus
                        .StaleSource));

            Assert.That(
                backend.MutationCalls,
                Is.Zero);

            Assert.That(
                ReadCurrentHead()
                    .currentGenerationId,
                Is.EqualTo(
                    intervening.GenerationId
                        .Value));
        }

        [Test]
        public void NewlyInstalledCanonicalOwnerBlocksUnknownCarryForward()
        {
            SourceState source =
                InstallSource();

            Register(
                Participant(
                    "com.example.inventory"));

            Register(
                Participant(
                    "com.example.future"));

            backend.ResetMutationCount();

            SaveCarryForwardPublicationResult result =
                PublishCarryForward(
                    source.Snapshot,
                    "{\"gold\":150}");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCarryForwardPublicationStatus
                        .OwnershipCollision));

            Assert.That(
                result.FailingPersistedId.Value,
                Is.EqualTo(
                    "com.example.future"));

            Assert.That(
                result.CurrentOwnerId.Value,
                Is.EqualTo(
                    "com.example.future"));

            Assert.That(
                backend.MutationCalls,
                Is.Zero);
        }

        [Test]
        public void NewlyInstalledAliasOwnerBlocksUnknownCarryForward()
        {
            SourceState source =
                InstallSource();

            Register(
                Participant(
                    "com.example.inventory"));

            Register(
                Participant(
                    "com.example.future2",
                    new SaveParticipantId(
                        "com.example.future")));

            backend.ResetMutationCount();

            SaveCarryForwardPublicationResult result =
                PublishCarryForward(
                    source.Snapshot,
                    "{\"gold\":150}");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCarryForwardPublicationStatus
                        .OwnershipCollision));

            Assert.That(
                result.CurrentOwnerId.Value,
                Is.EqualTo(
                    "com.example.future2"));

            Assert.That(
                backend.MutationCalls,
                Is.Zero);
        }

        [TestCase(FaultPoint.CandidatePayloadWrite)]
        [TestCase(FaultPoint.GenerationPublication)]
        [TestCase(FaultPoint.PublishedPayloadReadCorruption)]
        public void PreHeadPublicationFailuresPreserveSourceHead(
            FaultPoint fault)
        {
            SourceState source =
                InstallSource();

            Register(
                Participant(
                    "com.example.inventory"));

            byte[] oldHead =
                ReadCurrentHeadBytes();

            backend.Fault =
                fault;

            SaveCarryForwardPublicationResult result =
                PublishCarryForward(
                    source.Snapshot,
                    "{\"gold\":150}");

            Assert.That(
                result.Succeeded,
                Is.False);

            Assert.That(
                result.HeadPublished,
                Is.False);

            Assert.That(
                ReadCurrentHeadBytes(),
                Is.EqualTo(
                    oldHead));

            Assert.That(
                ReadCurrentHead()
                    .currentGenerationId,
                Is.EqualTo(
                    source.GenerationId.Value));
        }

        [Test]
        public void HeadPublicationFailureLeavesMergedGenerationOrphaned()
        {
            SourceState source =
                InstallSource();

            Register(
                Participant(
                    "com.example.inventory"));

            byte[] oldHead =
                ReadCurrentHeadBytes();

            backend.Fault =
                FaultPoint.HeadPublication;

            SaveCarryForwardPublicationResult result =
                PublishCarryForward(
                    source.Snapshot,
                    "{\"gold\":150}");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveCarryForwardPublicationStatus
                        .PublicationFailed));

            Assert.That(
                result.GenerationPublished,
                Is.True);

            Assert.That(
                result.HeadPublished,
                Is.False);

            Assert.That(
                ReadCurrentHeadBytes(),
                Is.EqualTo(
                    oldHead));

            SaveGenerationStorageKeys.TryCreate(
                slotId,
                result.PublishedGenerationId,
                out SaveGenerationStorageKeys orphanKeys);

            Assert.That(
                local.Read(
                    orphanKeys.GenerationManifest)
                    .Succeeded,
                Is.True);

            Assert.That(
                ReadCurrentHead()
                    .currentGenerationId,
                Is.EqualTo(
                    source.GenerationId.Value));
        }

        private void RebuildCoordinators()
        {
            publication =
                new SaveGenerationPublicationCoordinator(
                    backend,
                    serializer,
                    integrity,
                    clock,
                    SaveGenerationId.NewId);

            carryForward =
                new SaveUnknownPayloadCarryForwardCoordinator(
                    backend,
                    serializer,
                    integrity,
                    registry,
                    publication);
        }

        private SourceState InstallSource()
        {
            EntryPair unknown =
                Pair(
                    "com.example.future",
                    "{ \"future\" : \"  keep exactly  \" }",
                    7,
                    false);

            EntryPair known =
                Pair(
                    "com.example.inventory",
                    "{\"gold\":100}");

            SaveGenerationPublicationResult source =
                publication
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-source",
                        "Source",
                        Batch(
                            unknown,
                            known));

            Assert.That(
                source.Succeeded,
                Is.True);

            Assert.That(
                unknownStore.TryReplace(
                    new[]
                    {
                        unknown.Payload
                    },
                    slotId,
                    source.GenerationId)
                    .Succeeded,
                Is.True);

            return new SourceState(
                source.GenerationId,
                unknown,
                unknownStore.GetSnapshot());
        }

        private SaveCarryForwardPublicationResult
            PublishCarryForward(
                SaveUnknownPayloadSnapshot snapshot,
                string freshInventoryPayload) =>
            carryForward.PublishNextGeneration(
                slotId,
                "com.example.game",
                "1.0.0",
                "build-next",
                "Next",
                Batch(
                    Pair(
                        "com.example.inventory",
                        freshInventoryPayload)),
                snapshot);

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

        private void Register(
            TrackingParticipant participant)
        {
            Assert.That(
                registry.Register(
                    participant)
                    .Succeeded,
                Is.True);
        }

        private SaveParticipantCaptureBatchResult Batch(
            params EntryPair[] pairs)
        {
            Array.Sort(
                pairs,
                (left, right) =>
                    string.Compare(
                        left.Payload.participantId,
                        right.Payload.participantId,
                        StringComparison.Ordinal));

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

            SavePayloadEntry payload =
                new SavePayloadEntry
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

            SavePayloadInventoryEntry inventory =
                new SavePayloadInventoryEntry
                {
                    participantId =
                        participantId,
                    participantSchemaVersion =
                        schemaVersion,
                    serializerId =
                        payload.serializerId,
                    required =
                        required,
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

        private SaveHeadPointer ReadCurrentHead()
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

            Assert.That(
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SaveHeadPointer head)
                    .Succeeded,
                Is.True);

            return head;
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

        private readonly struct SourceState
        {
            internal SourceState(
                SaveGenerationId generationId,
                EntryPair unknown,
                SaveUnknownPayloadSnapshot snapshot)
            {
                GenerationId =
                    generationId;

                Unknown =
                    unknown;

                Snapshot =
                    snapshot;
            }

            internal SaveGenerationId GenerationId { get; }

            internal EntryPair Unknown { get; }

            internal SaveUnknownPayloadSnapshot Snapshot { get; }
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

            internal SavePayloadInventoryEntry Inventory { get; }
        }

        public enum FaultPoint
        {
            None = 0,
            CandidatePayloadWrite = 1,
            GenerationPublication = 2,
            PublishedPayloadReadCorruption = 3,
            HeadPublication = 4
        }

        private sealed class TrackingPublicationBackend :
            ISaveStoragePublicationBackend
        {
            private readonly LocalFileSaveStorageBackend inner;

            internal TrackingPublicationBackend(
                LocalFileSaveStorageBackend inner)
            {
                this.inner =
                    inner;
            }

            internal FaultPoint Fault { get; set; }

            internal int MutationCalls { get; private set; }

            internal void ResetMutationCount()
            {
                MutationCalls =
                    0;
            }

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

                bool corruptPublished =
                    Fault ==
                        FaultPoint
                            .PublishedPayloadReadCorruption &&
                    key.Value.Contains(
                        "/generations/") &&
                    key.Value.EndsWith(
                        "/payload.json",
                        StringComparison.Ordinal);

                if (!corruptPublished)
                {
                    return result;
                }

                byte[] data =
                    result.Data;

                if (data.Length > 0)
                {
                    data[
                        data.Length - 1] =
                        (byte)'!';
                }

                return new SaveStorageReadResult(
                    SaveStorageResult.Success(
                        "Fault-injected published payload corruption."),
                    data);
            }

            public SaveStorageResult WriteNew(
                SaveStorageKey key,
                byte[] data)
            {
                MutationCalls++;

                if (Fault ==
                        FaultPoint.CandidatePayloadWrite &&
                    key.Value.Contains(
                        "/incomplete/") &&
                    key.Value.EndsWith(
                        "/payload.json",
                        StringComparison.Ordinal))
                {
                    return InjectedFailure(
                        "Candidate payload write fault.");
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

            private static SaveStorageResult InjectedFailure(
                string message) =>
                new SaveStorageResult(
                    SaveStorageStatus.Failed,
                    "ESV-TEST-FAULT",
                    message);
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

        private sealed class TrackingParticipant :
            ISaveParticipant
        {
            internal TrackingParticipant(
                SaveParticipantDescriptor descriptor)
            {
                Descriptor =
                    descriptor;
            }

            public SaveParticipantDescriptor Descriptor { get; }

            public SaveParticipantCaptureResult Capture() =>
                SaveParticipantCaptureResult.Failure(
                    "Carry-forward publication must not invoke participant capture.");

            public SaveParticipantApplyResult Apply(
                object detachedState) =>
                SaveParticipantApplyResult.Failure(
                    "Carry-forward publication must not invoke participant apply.");
        }
    }
}
