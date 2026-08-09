
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantCaptureCoordinatorTests
    {
        private SaveParticipantRegistry registry;
        private SaveSerializerRegistry serializers;
        private Sha256IntegrityProvider integrity;

        [SetUp]
        public void SetUp()
        {
            registry =
                new SaveParticipantRegistry();

            serializers =
                new SaveSerializerRegistry();

            integrity =
                new Sha256IntegrityProvider();
        }

        [Test]
        public void EmptyRegistryProducesEmptySuccessfulBatch()
        {
            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Count,
                Is.Zero);

            Assert.That(
                result.TotalPayloadBytes,
                Is.Zero);
        }

        [Test]
        public void CaptureOrderUsesCanonicalParticipantId()
        {
            registry.Register(
                Participant(
                    "com.example.zeta",
                    30));

            registry.Register(
                Participant(
                    "com.example.alpha",
                    10));

            registry.Register(
                Participant(
                    "com.example.middle",
                    20));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.PayloadEntries[0]
                    .participantId,
                Is.EqualTo(
                    "com.example.alpha"));

            Assert.That(
                result.PayloadEntries[1]
                    .participantId,
                Is.EqualTo(
                    "com.example.middle"));

            Assert.That(
                result.PayloadEntries[2]
                    .participantId,
                Is.EqualTo(
                    "com.example.zeta"));
        }

        [Test]
        public void DefaultSerializerResolvesUnityJson()
        {
            registry.Register(
                Participant(
                    "com.example.inventory",
                    100));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.PayloadEntries[0]
                    .serializerId,
                Is.EqualTo(
                    UnityJsonSaveSerializer
                        .StableId));
        }

        [Test]
        public void ExplicitRuntimeSerializerIsUsed()
        {
            RecordingRuntimeSerializer serializer =
                new RecordingRuntimeSerializer(
                    "tests.runtime");

            serializers.TryRegister(
                serializer);

            registry.Register(
                Participant(
                    "com.example.inventory",
                    100,
                    SaveParticipantCriticality.Required,
                    new SaveSerializerId(
                        "tests.runtime")));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                serializer.SerializeCalls,
                Is.EqualTo(1));

            Assert.That(
                result.PayloadEntries[0]
                    .serializerId,
                Is.EqualTo(
                    "tests.runtime"));
        }

        [Test]
        public void PayloadByteLengthMatchesExactUtf8Bytes()
        {
            registry.Register(
                Participant(
                    "com.example.inventory",
                    100,
                    label: "Potion ⚗"));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            SavePayloadEntry entry =
                result.PayloadEntries[0];

            Assert.That(
                entry.byteLength,
                Is.EqualTo(
                    Encoding.UTF8
                        .GetByteCount(
                            entry.serializedPayload)));
        }

        [Test]
        public void EntryChecksumMatchesExactSerializedUtf8Bytes()
        {
            registry.Register(
                Participant(
                    "com.example.inventory",
                    100));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            SavePayloadEntry entry =
                result.PayloadEntries[0];

            SaveIntegrityResult verified =
                integrity.Verify(
                    Encoding.UTF8.GetBytes(
                        entry.serializedPayload),
                    entry.checksum);

            Assert.That(
                verified.Succeeded,
                Is.True);
        }

        [Test]
        public void PayloadAndInventoryMetadataAgree()
        {
            registry.Register(
                Participant(
                    "com.example.inventory",
                    100,
                    SaveParticipantCriticality.Optional));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            SavePayloadEntry payload =
                result.PayloadEntries[0];

            SavePayloadInventoryEntry inventory =
                result.InventoryEntries[0];

            Assert.That(
                inventory.participantId,
                Is.EqualTo(
                    payload.participantId));

            Assert.That(
                inventory.participantSchemaVersion,
                Is.EqualTo(
                    payload.participantSchemaVersion));

            Assert.That(
                inventory.serializerId,
                Is.EqualTo(
                    payload.serializerId));

            Assert.That(
                inventory.required,
                Is.EqualTo(
                    payload.required));

            Assert.That(
                inventory.byteLength,
                Is.EqualTo(
                    payload.byteLength));

            Assert.That(
                inventory.checksum,
                Is.EqualTo(
                    payload.checksum));

            Assert.That(
                inventory.flags,
                Is.EqualTo(
                    payload.flags));
        }

        [Test]
        public void RequiredAndOptionalProjectIntoTransportFlag()
        {
            registry.Register(
                Participant(
                    "com.example.required",
                    1,
                    SaveParticipantCriticality.Required));

            registry.Register(
                Participant(
                    "com.example.optional",
                    2,
                    SaveParticipantCriticality.Optional));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            Dictionary<string, bool> required =
                new Dictionary<string, bool>();

            for (int i = 0;
                 i < result.Count;
                 i++)
            {
                required[
                    result.PayloadEntries[i]
                        .participantId] =
                    result.PayloadEntries[i]
                        .required;
            }

            Assert.That(
                required[
                    "com.example.required"],
                Is.True);

            Assert.That(
                required[
                    "com.example.optional"],
                Is.False);
        }

        [Test]
        public void CaptureFailureAbortsWholeBatch()
        {
            registry.Register(
                Participant(
                    "com.example.alpha",
                    1));

            registry.Register(
                new TypedParticipant(
                    Descriptor(
                        "com.example.beta"),
                    typeof(TestDto),
                    () =>
                        SaveParticipantCaptureResult
                            .Failure(
                                "No snapshot.")));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            AssertFailedEmpty(
                result,
                SaveParticipantCaptureBatchStatus
                    .CaptureFailed);
        }

        [Test]
        public void NullSuccessfulCaptureAbortsWholeBatch()
        {
            registry.Register(
                new TypedParticipant(
                    Descriptor(
                        "com.example.inventory"),
                    typeof(TestDto),
                    () =>
                        SaveParticipantCaptureResult
                            .Success(
                                null)));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            AssertFailedEmpty(
                result,
                SaveParticipantCaptureBatchStatus
                    .DetachedStateInvalid);
        }

        [Test]
        public void WrongDetachedStateTypeAbortsWholeBatch()
        {
            registry.Register(
                new TypedParticipant(
                    Descriptor(
                        "com.example.inventory"),
                    typeof(TestDto),
                    () =>
                        SaveParticipantCaptureResult
                            .Success(
                                new OtherDto())));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            AssertFailedEmpty(
                result,
                SaveParticipantCaptureBatchStatus
                    .DetachedStateInvalid);
        }

        [Test]
        public void UntypedParticipantAbortsCaptureBatch()
        {
            registry.Register(
                new UntypedParticipant(
                    Descriptor(
                        "com.example.inventory")));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            AssertFailedEmpty(
                result,
                SaveParticipantCaptureBatchStatus
                    .DetachedStateInvalid);
        }

        [Test]
        public void MissingSerializerAbortsWholeBatch()
        {
            registry.Register(
                Participant(
                    "com.example.inventory",
                    1,
                    SaveParticipantCriticality.Required,
                    new SaveSerializerId(
                        "tests.missing")));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            AssertFailedEmpty(
                result,
                SaveParticipantCaptureBatchStatus
                    .SerializerUnavailable);
        }

        [Test]
        public void SerializerWithoutRuntimeTypeCapabilityAbortsBatch()
        {
            serializers.TryRegister(
                new GenericOnlySerializer(
                    "tests.generic-only"));

            registry.Register(
                Participant(
                    "com.example.inventory",
                    1,
                    SaveParticipantCriticality.Required,
                    new SaveSerializerId(
                        "tests.generic-only")));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            AssertFailedEmpty(
                result,
                SaveParticipantCaptureBatchStatus
                    .SerializerUnavailable);
        }

        [Test]
        public void SerializerFailureAbortsWholeBatch()
        {
            serializers.TryRegister(
                new FailingRuntimeSerializer(
                    "tests.fail"));

            registry.Register(
                Participant(
                    "com.example.inventory",
                    1,
                    SaveParticipantCriticality.Required,
                    new SaveSerializerId(
                        "tests.fail")));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            AssertFailedEmpty(
                result,
                SaveParticipantCaptureBatchStatus
                    .SerializationFailed);
        }

        [Test]
        public void IntegrityFailureAbortsWholeBatch()
        {
            registry.Register(
                Participant(
                    "com.example.inventory",
                    1));

            SaveParticipantCaptureCoordinator coordinator =
                new SaveParticipantCaptureCoordinator(
                    serializers,
                    new FailingIntegrityProvider());

            SaveParticipantCaptureBatchResult result =
                coordinator.Capture(
                    registry);

            AssertFailedEmpty(
                result,
                SaveParticipantCaptureBatchStatus
                    .IntegrityFailed);
        }

        [Test]
        public void FutureParticipantUsesSameCapturePipeline()
        {
            registry.Register(
                Participant(
                    "com.echodevgames.echo-pets",
                    313,
                    label: "Space Lobster"));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.PayloadEntries[0]
                    .participantId,
                Is.EqualTo(
                    "com.echodevgames.echo-pets"));

            Assert.That(
                result.PayloadEntries[0]
                    .serializedPayload,
                Does.Contain(
                    "Space Lobster"));
        }

        [Test]
        public void BatchEntryAccessIsDefensivelyCopied()
        {
            registry.Register(
                Participant(
                    "com.example.inventory",
                    100));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            SavePayloadEntry firstRead =
                result.PayloadEntries[0];

            firstRead.participantId =
                "com.example.mutated";

            Assert.That(
                result.PayloadEntries[0]
                    .participantId,
                Is.EqualTo(
                    "com.example.inventory"));
        }

        [Test]
        public void CaptureCoordinatorDoesNotTouchFilesystem()
        {
            string sentinel =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-M3-02-NoStorage-" +
                    Guid.NewGuid()
                        .ToString("N"));

            registry.Register(
                Participant(
                    "com.example.inventory",
                    100));

            SaveParticipantCaptureBatchResult result =
                CreateCoordinator()
                    .Capture(
                        registry);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                Directory.Exists(
                    sentinel),
                Is.False);

            Assert.That(
                File.Exists(
                    sentinel),
                Is.False);
        }

        private SaveParticipantCaptureCoordinator
            CreateCoordinator() =>
            new SaveParticipantCaptureCoordinator(
                serializers,
                integrity);

        private static TypedParticipant Participant(
            string id,
            int value,
            SaveParticipantCriticality criticality =
                SaveParticipantCriticality.Required,
            SaveSerializerId serializerId =
                default,
            string label =
                "Chronicle") =>
            new TypedParticipant(
                Descriptor(
                    id,
                    criticality,
                    serializerId),
                typeof(TestDto),
                () =>
                    SaveParticipantCaptureResult
                        .Success(
                            new TestDto
                            {
                                value =
                                    value,
                                label =
                                    label
                            }));

        private static SaveParticipantDescriptor
            Descriptor(
                string id,
                SaveParticipantCriticality criticality =
                    SaveParticipantCriticality.Required,
                SaveSerializerId serializerId =
                    default) =>
            new SaveParticipantDescriptor(
                new SaveParticipantId(
                    id),
                3,
                criticality,
                SaveMissingPayloadPolicy
                    .InitializeDefault,
                serializerId);

        private static void AssertFailedEmpty(
            SaveParticipantCaptureBatchResult result,
            SaveParticipantCaptureBatchStatus expected)
        {
            Assert.That(
                result.Status,
                Is.EqualTo(
                    expected));

            Assert.That(
                result.Succeeded,
                Is.False);

            Assert.That(
                result.Count,
                Is.Zero);

            Assert.That(
                result.PayloadEntries.Count,
                Is.Zero);

            Assert.That(
                result.InventoryEntries.Count,
                Is.Zero);

            Assert.That(
                result.TotalPayloadBytes,
                Is.Zero);
        }

        [Serializable]
        private sealed class TestDto
        {
            public int value;
            public string label;
        }

        [Serializable]
        private sealed class OtherDto
        {
            public string other;
        }

        private sealed class TypedParticipant :
            ISaveTypedParticipant
        {
            private readonly
                Func<SaveParticipantCaptureResult>
                    capture;

            internal TypedParticipant(
                SaveParticipantDescriptor descriptor,
                Type detachedStateType,
                Func<SaveParticipantCaptureResult> capture)
            {
                Descriptor =
                    descriptor;
                DetachedStateType =
                    detachedStateType;
                this.capture =
                    capture;
            }

            public SaveParticipantDescriptor Descriptor
            {
                get;
            }

            public Type DetachedStateType { get; }

            public SaveParticipantCaptureResult Capture() =>
                capture();

            public SaveParticipantApplyResult Apply(
                object detachedState) =>
                SaveParticipantApplyResult.Success();
        }

        private sealed class UntypedParticipant :
            ISaveParticipant
        {
            internal UntypedParticipant(
                SaveParticipantDescriptor descriptor)
            {
                Descriptor =
                    descriptor;
            }

            public SaveParticipantDescriptor Descriptor
            {
                get;
            }

            public SaveParticipantCaptureResult Capture() =>
                SaveParticipantCaptureResult.Success(
                    new TestDto());

            public SaveParticipantApplyResult Apply(
                object detachedState) =>
                SaveParticipantApplyResult.Success();
        }

        private sealed class RecordingRuntimeSerializer :
            IRuntimeTypeSaveSerializer
        {
            private readonly
                UnityJsonSaveSerializer inner =
                    new UnityJsonSaveSerializer();

            internal RecordingRuntimeSerializer(
                string id)
            {
                Id =
                    new SaveSerializerId(
                        id);
            }

            public SaveSerializerId Id { get; }

            internal int SerializeCalls { get; private set; }

            public SaveSerializerResult Serialize<T>(
                T value,
                out string serialized) =>
                Serialize(
                    value,
                    typeof(T),
                    out serialized);

            public SaveSerializerResult Deserialize<T>(
                string serialized,
                out T value)
            {
                value =
                    default;

                SaveSerializerResult result =
                    Deserialize(
                        serialized,
                        typeof(T),
                        out object restored);

                if (result.Succeeded)
                {
                    value =
                        (T)restored;
                }

                return result;
            }

            public SaveSerializerResult Serialize(
                object value,
                Type valueType,
                out string serialized)
            {
                SerializeCalls++;

                return inner.Serialize(
                    value,
                    valueType,
                    out serialized);
            }

            public SaveSerializerResult Deserialize(
                string serialized,
                Type valueType,
                out object value) =>
                inner.Deserialize(
                    serialized,
                    valueType,
                    out value);
        }

        private sealed class GenericOnlySerializer :
            ISaveSerializer
        {
            internal GenericOnlySerializer(
                string id)
            {
                Id =
                    new SaveSerializerId(
                        id);
            }

            public SaveSerializerId Id { get; }

            public SaveSerializerResult Serialize<T>(
                T value,
                out string serialized)
            {
                serialized =
                    "{}";

                return SaveSerializerResult.Success(
                    "Generic-only test serializer.");
            }

            public SaveSerializerResult Deserialize<T>(
                string serialized,
                out T value)
            {
                value =
                    default;

                return SaveSerializerResult.Success(
                    "Generic-only test serializer.");
            }
        }

        private sealed class FailingRuntimeSerializer :
            IRuntimeTypeSaveSerializer
        {
            internal FailingRuntimeSerializer(
                string id)
            {
                Id =
                    new SaveSerializerId(
                        id);
            }

            public SaveSerializerId Id { get; }

            public SaveSerializerResult Serialize<T>(
                T value,
                out string serialized) =>
                Serialize(
                    value,
                    typeof(T),
                    out serialized);

            public SaveSerializerResult Deserialize<T>(
                string serialized,
                out T value)
            {
                value =
                    default;

                return new SaveSerializerResult(
                    SaveSerializerStatus.Failed,
                    "ESV-TEST-SERIAL",
                    "Injected serializer failure.");
            }

            public SaveSerializerResult Serialize(
                object value,
                Type valueType,
                out string serialized)
            {
                serialized =
                    string.Empty;

                return new SaveSerializerResult(
                    SaveSerializerStatus.Failed,
                    "ESV-TEST-SERIAL",
                    "Injected serializer failure.");
            }

            public SaveSerializerResult Deserialize(
                string serialized,
                Type valueType,
                out object value)
            {
                value =
                    null;

                return new SaveSerializerResult(
                    SaveSerializerStatus.Failed,
                    "ESV-TEST-SERIAL",
                    "Injected serializer failure.");
            }
        }

        private sealed class FailingIntegrityProvider :
            IIntegrityProvider
        {
            public SaveIntegrityProviderId Id =>
                new SaveIntegrityProviderId(
                    "tests.integrity-failure");

            public SaveIntegrityResult Calculate(
                byte[] data,
                out string checksum)
            {
                checksum =
                    string.Empty;

                return new SaveIntegrityResult(
                    SaveIntegrityStatus.Failed,
                    "ESV-TEST-INTEGRITY",
                    "Injected integrity failure.");
            }

            public SaveIntegrityResult Verify(
                byte[] data,
                string expectedChecksum) =>
                new SaveIntegrityResult(
                    SaveIntegrityStatus.Failed,
                    "ESV-TEST-INTEGRITY",
                    "Injected integrity failure.");
        }
    }
}
