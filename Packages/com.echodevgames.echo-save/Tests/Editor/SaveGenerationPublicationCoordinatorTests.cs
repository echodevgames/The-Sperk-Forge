
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveGenerationPublicationCoordinatorTests
    {
        private string sandboxParent;
        private string backendRoot;
        private LocalFileSaveStorageBackend local;
        private FaultingPublicationBackend backend;
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
                    "EchoSave-M2-04-Publish-" +
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
                new FaultingPublicationBackend(
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
                        17,
                        45,
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

            if (File.Exists(
                    sandboxParent))
            {
                File.Delete(
                    sandboxParent);
            }
        }

        [Test]
        public void FirstPublicationPublishesGenerationBeforeHead()
        {
            SaveGenerationPublicationCoordinator coordinator =
                CreateCoordinator();

            SaveGenerationPublicationResult result =
                coordinator.PublishEmptyTransportGeneration(
                    slotId,
                    "com.example.game",
                    "1.0.0",
                    "build-a",
                    "First");

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

            Assert.That(
                local.Read(
                    keys.GenerationPayload)
                    .Succeeded,
                Is.True);

            Assert.That(
                local.Read(
                    keys.GenerationManifest)
                    .Succeeded,
                Is.True);

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

            Assert.That(
                head.updateSequence,
                Is.EqualTo(1));
        }

        [Test]
        public void SecondPublicationAdvancesHeadAndPreservesPreviousGeneration()
        {
            SaveGenerationPublicationCoordinator coordinator =
                CreateCoordinator();

            SaveGenerationPublicationResult first =
                coordinator.PublishEmptyTransportGeneration(
                    slotId,
                    "com.example.game",
                    "1.0.0",
                    "build-a",
                    "First");

            clock.Utc =
                clock.Utc.AddMinutes(1);

            SaveGenerationPublicationResult second =
                coordinator.PublishEmptyTransportGeneration(
                    slotId,
                    "com.example.game",
                    "1.0.0",
                    "build-b",
                    "Second");

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
                    firstKeys.GenerationManifest)
                    .Succeeded,
                Is.True);
        }

        [Test]
        public void CandidatePayloadWriteFailureLeavesOldHeadUnchanged()
        {
            AssertPreHeadFailurePreservesOldHead(
                FaultPoint.CandidatePayloadWrite);
        }

        [Test]
        public void CandidateManifestWriteFailureLeavesOldHeadUnchanged()
        {
            AssertPreHeadFailurePreservesOldHead(
                FaultPoint.CandidateManifestWrite);
        }

        [Test]
        public void CandidateVerificationFailureLeavesOldHeadUnchanged()
        {
            AssertPreHeadFailurePreservesOldHead(
                FaultPoint.CandidatePayloadReadCorruption);
        }

        [Test]
        public void GenerationPublicationFailureLeavesOldHeadUnchanged()
        {
            AssertPreHeadFailurePreservesOldHead(
                FaultPoint.GenerationPublication);
        }

        [Test]
        public void HeadPublicationFailureLeavesOrphanGenerationAndOldHeadCurrent()
        {
            SaveGenerationPublicationCoordinator coordinator =
                CreateCoordinator();

            SaveGenerationPublicationResult first =
                coordinator.PublishEmptyTransportGeneration(
                    slotId,
                    "com.example.game",
                    "1.0.0",
                    "build-a",
                    "First");

            byte[] oldHead =
                ReadHeadBytes(
                    first.GenerationId);

            backend.Fault =
                FaultPoint.HeadPublication;

            SaveGenerationPublicationResult second =
                coordinator.PublishEmptyTransportGeneration(
                    slotId,
                    "com.example.game",
                    "1.0.0",
                    "build-b",
                    "Second");

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
                ReadHeadBytes(
                    first.GenerationId),
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

            SaveHeadPointer current =
                ReadHead(
                    secondKeys.Head);

            Assert.That(
                current.currentGenerationId,
                Is.EqualTo(
                    first.GenerationId.Value));
        }

        [Test]
        public void HeadSerializationFailureLeavesOrphanGenerationAndOldHeadCurrent()
        {
            SaveGenerationPublicationCoordinator initialCoordinator =
                CreateCoordinator();

            SaveGenerationPublicationResult first =
                initialCoordinator
                    .PublishEmptyTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "build-a",
                        "First");

            byte[] oldHead =
                ReadHeadBytes(
                    first.GenerationId);

            FaultingHeadSerializer faultingSerializer =
                new FaultingHeadSerializer(
                    serializer);

            SaveGenerationPublicationCoordinator coordinator =
                new SaveGenerationPublicationCoordinator(
                    backend,
                    faultingSerializer,
                    integrity,
                    clock,
                    SaveGenerationId.NewId);

            SaveGenerationPublicationResult second =
                coordinator.PublishEmptyTransportGeneration(
                    slotId,
                    "com.example.game",
                    "1.0.0",
                    "build-b",
                    "Second");

            Assert.That(
                second.Status,
                Is.EqualTo(
                    SaveGenerationPublicationStatus
                        .HeadPublicationFailed));

            Assert.That(
                second.GenerationPublished,
                Is.True);

            Assert.That(
                ReadHeadBytes(
                    first.GenerationId),
                Is.EqualTo(
                    oldHead));
        }

        [Test]
        public void DuplicateGenerationIdIsRejectedWithoutChangingHead()
        {
            SaveGenerationId fixedGeneration =
                SaveGenerationId.NewId();

            SaveGenerationPublicationCoordinator coordinator =
                new SaveGenerationPublicationCoordinator(
                    backend,
                    serializer,
                    integrity,
                    clock,
                    () => fixedGeneration);

            SaveGenerationPublicationResult first =
                coordinator.PublishEmptyTransportGeneration(
                    slotId,
                    "com.example.game",
                    "1.0.0",
                    "build-a",
                    "First");

            byte[] oldHead =
                ReadHeadBytes(
                    first.GenerationId);

            SaveGenerationPublicationResult duplicate =
                coordinator.PublishEmptyTransportGeneration(
                    slotId,
                    "com.example.game",
                    "1.0.0",
                    "build-b",
                    "Duplicate");

            Assert.That(
                duplicate.Status,
                Is.EqualTo(
                    SaveGenerationPublicationStatus
                        .GenerationPublicationFailed));

            Assert.That(
                duplicate.GenerationPublished,
                Is.False);

            Assert.That(
                ReadHeadBytes(
                    first.GenerationId),
                Is.EqualTo(
                    oldHead));
        }

        [Test]
        public void PublishedGenerationRemainsCreateOnlyImmutable()
        {
            SaveGenerationPublicationCoordinator coordinator =
                CreateCoordinator();

            SaveGenerationPublicationResult result =
                coordinator.PublishEmptyTransportGeneration(
                    slotId,
                    "com.example.game",
                    "1.0.0",
                    "build-a",
                    "First");

            SaveGenerationStorageKeys.TryCreate(
                slotId,
                result.GenerationId,
                out SaveGenerationStorageKeys keys);

            SaveStorageResult overwrite =
                local.WriteNew(
                    keys.GenerationPayload,
                    new byte[] { 99 });

            Assert.That(
                overwrite.Status,
                Is.EqualTo(
                    SaveStorageStatus.Conflict));
        }

        private void AssertPreHeadFailurePreservesOldHead(
            FaultPoint fault)
        {
            SaveGenerationPublicationCoordinator coordinator =
                CreateCoordinator();

            SaveGenerationPublicationResult first =
                coordinator.PublishEmptyTransportGeneration(
                    slotId,
                    "com.example.game",
                    "1.0.0",
                    "build-a",
                    "First");

            Assert.That(
                first.Succeeded,
                Is.True);

            byte[] oldHead =
                ReadHeadBytes(
                    first.GenerationId);

            backend.Fault =
                fault;

            SaveGenerationPublicationResult second =
                coordinator.PublishEmptyTransportGeneration(
                    slotId,
                    "com.example.game",
                    "1.0.0",
                    "build-b",
                    "Second");

            Assert.That(
                second.Succeeded,
                Is.False);

            Assert.That(
                second.HeadPublished,
                Is.False);

            Assert.That(
                ReadHeadBytes(
                    first.GenerationId),
                Is.EqualTo(
                    oldHead));

            SaveHeadPointer current =
                ReadCurrentHead();

            Assert.That(
                current.currentGenerationId,
                Is.EqualTo(
                    first.GenerationId.Value));
        }

        private SaveGenerationPublicationCoordinator
            CreateCoordinator() =>
            new SaveGenerationPublicationCoordinator(
                backend,
                serializer,
                integrity,
                clock,
                SaveGenerationId.NewId);

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

        private byte[] ReadHeadBytes(
            SaveGenerationId anyGeneration)
        {
            SaveGenerationStorageKeys.TryCreate(
                slotId,
                anyGeneration,
                out SaveGenerationStorageKeys keys);

            return local.Read(
                keys.Head)
                .Data;
        }

        private SaveHeadPointer ReadHead(
            SaveStorageKey headKey)
        {
            SaveStorageReadResult read =
                local.Read(
                    headKey);

            Assert.That(
                read.Succeeded,
                Is.True);

            SaveSerializerResult deserialized =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SaveHeadPointer head);

            Assert.That(
                deserialized.Succeeded,
                Is.True);

            return head;
        }

        private enum FaultPoint
        {
            None = 0,
            CandidatePayloadWrite = 1,
            CandidateManifestWrite = 2,
            CandidatePayloadReadCorruption = 3,
            GenerationPublication = 4,
            HeadPublication = 5
        }

        private sealed class FaultingPublicationBackend :
            ISaveStoragePublicationBackend
        {
            private readonly
                LocalFileSaveStorageBackend inner;

            internal FaultingPublicationBackend(
                LocalFileSaveStorageBackend inner)
            {
                this.inner = inner;
            }

            internal FaultPoint Fault
            {
                get;
                set;
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

                if (Fault ==
                        FaultPoint
                            .CandidatePayloadReadCorruption &&
                    result.Succeeded &&
                    key.Value.Contains(
                        "/incomplete/") &&
                    key.Value.EndsWith(
                        "/payload.json",
                        StringComparison.Ordinal))
                {
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
                            "Fault-injected corrupted candidate read."),
                        corrupted);
                }

                return result;
            }

            public SaveStorageResult WriteNew(
                SaveStorageKey key,
                byte[] data)
            {
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
                SaveStorageKey key) =>
                inner.Delete(
                    key);

            public SaveStorageResult PublishNewTree(
                SaveStorageKey sourceDirectoryKey,
                SaveStorageKey destinationDirectoryKey)
            {
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
                this.inner = inner;
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
                Utc = utc;
            }

            internal DateTime Utc
            {
                get;
                set;
            }

            public DateTime UtcNow =>
                Utc;

            public double MonotonicSeconds =>
                0d;
        }
    }
}
