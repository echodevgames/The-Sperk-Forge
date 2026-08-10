using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantPayloadPreparerTests
    {
        private SaveParticipantRegistry registry;
        private SaveSerializerRegistry serializers;
        private SaveSlotId slotId;
        private SaveGenerationId generationId;

        [SetUp]
        public void SetUp()
        {
            registry =
                new SaveParticipantRegistry();

            serializers =
                new SaveSerializerRegistry();

            slotId =
                SaveSlotId.NewId();

            generationId =
                SaveGenerationId.NewId();
        }

        [Test]
        public void CurrentVersionCanonicalParticipantPreparesSuccessfully()
        {
            TypedParticipant participant =
                RegisterTyped(
                    "com.example.inventory",
                    1);

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                "{\"value\":42}")));

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.PreparedCount,
                Is.EqualTo(1));

            SavePreparedParticipantEntry prepared =
                result.Batch.Entries[0];

            Assert.That(
                prepared.PersistedParticipantId.Value,
                Is.EqualTo(
                    "com.example.inventory"));

            Assert.That(
                prepared.CanonicalParticipantId.Value,
                Is.EqualTo(
                    "com.example.inventory"));

            Assert.That(
                prepared.DetachedState,
                Is.TypeOf<TestState>());

            Assert.That(
                ((TestState)prepared.DetachedState).value,
                Is.EqualTo(42));

            Assert.That(
                participant.CaptureCalls,
                Is.Zero);

            Assert.That(
                participant.ApplyCalls,
                Is.Zero);
        }

        [Test]
        public void MultipleParticipantsPrepareInCanonicalOwnerOrder()
        {
            RegisterTyped(
                "com.example.zeta",
                1);

            RegisterTyped(
                "com.example.alpha",
                1);

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.zeta",
                                "{\"value\":2}"),
                            Entry(
                                "com.example.alpha",
                                "{\"value\":1}")));

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Batch.Entries[0]
                    .CanonicalParticipantId.Value,
                Is.EqualTo(
                    "com.example.alpha"));

            Assert.That(
                result.Batch.Entries[1]
                    .CanonicalParticipantId.Value,
                Is.EqualTo(
                    "com.example.zeta"));
        }

        [Test]
        public void PersistedAliasResolvesCanonicalOwnerAndRetainsPersistedId()
        {
            RegisterTyped(
                "com.example.inventory",
                1,
                new SaveParticipantId(
                    "com.example.oldinventory"));

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.oldinventory",
                                "{\"value\":7}")));

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Batch.Entries[0]
                    .PersistedParticipantId.Value,
                Is.EqualTo(
                    "com.example.oldinventory"));

            Assert.That(
                result.Batch.Entries[0]
                    .CanonicalParticipantId.Value,
                Is.EqualTo(
                    "com.example.inventory"));
        }

        [Test]
        public void UnknownParticipantNeverResolvesItsSerializer()
        {
            CountingRuntimeSerializer spy =
                new CountingRuntimeSerializer(
                    "tests.unknown-spy");

            Assert.That(
                serializers.TryRegister(
                    spy)
                    .Succeeded,
                Is.True);

            SavePayloadEntry unknown =
                Entry(
                    "com.example.future",
                    "{\"value\":99}");

            unknown.serializerId =
                spy.Id.Value;

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            unknown));

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.PreparedCount,
                Is.Zero);

            Assert.That(
                spy.DeserializeCalls,
                Is.Zero);
        }

        [Test]
        public void ParticipantWithoutTypedCapabilityFailsClosed()
        {
            UntypedParticipant participant =
                new UntypedParticipant(
                    Descriptor(
                        "com.example.inventory",
                        1));

            Assert.That(
                registry.Register(
                    participant)
                    .Succeeded,
                Is.True);

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                "{\"value\":1}")));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantPreparationStatus
                        .RuntimeTypeUnavailable));

            Assert.That(
                result.Batch,
                Is.Null);
        }

        [Test]
        public void NullTrustedRuntimeTypeFailsClosed()
        {
            TypedParticipant participant =
                new TypedParticipant(
                    Descriptor(
                        "com.example.inventory",
                        1),
                    null);

            Assert.That(
                registry.Register(
                    participant)
                    .Succeeded,
                Is.True);

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                "{\"value\":1}")));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantPreparationStatus
                        .RuntimeTypeUnavailable));
        }

        [Test]
        public void OlderSchemaReturnsMigrationRequired()
        {
            RegisterTyped(
                "com.example.inventory",
                2);

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                "{\"value\":1}",
                                1)));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantPreparationStatus
                        .MigrationRequired));

            Assert.That(
                result.Batch,
                Is.Null);
        }

        [Test]
        public void NewerSchemaReturnsUnsupportedNewer()
        {
            RegisterTyped(
                "com.example.inventory",
                1);

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                "{\"value\":1}",
                                2)));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantPreparationStatus
                        .NewerSchemaUnsupported));
        }

        [Test]
        public void MissingSerializerProviderFailsClosed()
        {
            RegisterTyped(
                "com.example.inventory",
                1);

            SavePayloadEntry entry =
                Entry(
                    "com.example.inventory",
                    "{\"value\":1}");

            entry.serializerId =
                "tests.missing-provider";

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            entry));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantPreparationStatus
                        .SerializerUnavailable));
        }

        [Test]
        public void SerializerWithoutRuntimeTypeCapabilityFailsClosed()
        {
            RegisterTyped(
                "com.example.inventory",
                1);

            NonRuntimeSerializer provider =
                new NonRuntimeSerializer();

            Assert.That(
                serializers.TryRegister(
                    provider)
                    .Succeeded,
                Is.True);

            SavePayloadEntry entry =
                Entry(
                    "com.example.inventory",
                    "{\"value\":1}");

            entry.serializerId =
                provider.Id.Value;

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            entry));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantPreparationStatus
                        .SerializerUnavailable));
        }

        [Test]
        public void AlternateRegisteredRuntimeSerializerIsUsed()
        {
            RegisterTyped(
                "com.example.inventory",
                1);

            CountingRuntimeSerializer provider =
                new CountingRuntimeSerializer(
                    "tests.runtime");

            Assert.That(
                serializers.TryRegister(
                    provider)
                    .Succeeded,
                Is.True);

            SavePayloadEntry entry =
                Entry(
                    "com.example.inventory",
                    "{\"value\":1}");

            entry.serializerId =
                provider.Id.Value;

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            entry));

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                provider.DeserializeCalls,
                Is.EqualTo(1));

            Assert.That(
                ((TestState)result.Batch
                    .Entries[0]
                    .DetachedState)
                    .value,
                Is.EqualTo(77));
        }

        [Test]
        public void MalformedJsonFailsWithoutPreparedBatch()
        {
            RegisterTyped(
                "com.example.inventory",
                1);

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                "{not-json")));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantPreparationStatus
                        .DeserializationFailed));

            Assert.That(
                result.Batch,
                Is.Null);

            Assert.That(
                result.PreparedCount,
                Is.Zero);
        }

        [Test]
        public void SuccessfulProviderReturningNullFailsClosed()
        {
            RegisterTyped(
                "com.example.inventory",
                1);

            InvalidStateRuntimeSerializer provider =
                new InvalidStateRuntimeSerializer();

            Assert.That(
                serializers.TryRegister(
                    provider)
                    .Succeeded,
                Is.True);

            SavePayloadEntry entry =
                Entry(
                    "com.example.inventory",
                    "{\"value\":1}");

            entry.serializerId =
                provider.Id.Value;

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            entry));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantPreparationStatus
                        .DetachedStateInvalid));

            Assert.That(
                result.Batch,
                Is.Null);
        }

        [Test]
        public void CanonicalAndAliasEntriesForSameOwnerFailAsDuplicateOwner()
        {
            RegisterTyped(
                "com.example.inventory",
                1,
                new SaveParticipantId(
                    "com.example.oldinventory"));

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                "{\"value\":1}"),
                            Entry(
                                "com.example.oldinventory",
                                "{\"value\":2}")));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantPreparationStatus
                        .DuplicateCanonicalOwner));

            Assert.That(
                result.Batch,
                Is.Null);
        }

        [Test]
        public void LaterParticipantFailureExposesNoPartialBatch()
        {
            RegisterTyped(
                "com.example.alpha",
                1);

            RegisterTyped(
                "com.example.beta",
                1);

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.alpha",
                                "{\"value\":1}"),
                            Entry(
                                "com.example.beta",
                                "{bad")));

            Assert.That(
                result.Succeeded,
                Is.False);

            Assert.That(
                result.Batch,
                Is.Null);

            Assert.That(
                result.PreparedCount,
                Is.Zero);
        }

        [Test]
        public void PreparedBatchPreservesSourceIdentity()
        {
            RegisterTyped(
                "com.example.inventory",
                1);

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                "{\"value\":1}")));

            Assert.That(
                result.Batch.SourceSlotId,
                Is.EqualTo(
                    slotId));

            Assert.That(
                result.Batch.SourceGenerationId,
                Is.EqualTo(
                    generationId));
        }

        [Test]
        public void PreparedBatchEntryArrayIsDefensive()
        {
            RegisterTyped(
                "com.example.inventory",
                1);

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                "{\"value\":1}")));

            IReadOnlyList<SavePreparedParticipantEntry>
                first =
                    result.Batch.Entries;

            Assert.That(
                first.Count,
                Is.EqualTo(1));

            Assert.That(
                result.Batch.Entries.Count,
                Is.EqualTo(1));
        }

        [Test]
        public void PreparationNeverInvokesCaptureOrApply()
        {
            TypedParticipant alpha =
                RegisterTyped(
                    "com.example.alpha",
                    1);

            TypedParticipant beta =
                RegisterTyped(
                    "com.example.beta",
                    1);

            SaveParticipantPreparationResult result =
                CreatePreparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.alpha",
                                "{\"value\":1}"),
                            Entry(
                                "com.example.beta",
                                "{\"value\":2}")));

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                alpha.CaptureCalls,
                Is.Zero);

            Assert.That(
                alpha.ApplyCalls,
                Is.Zero);

            Assert.That(
                beta.CaptureCalls,
                Is.Zero);

            Assert.That(
                beta.ApplyCalls,
                Is.Zero);
        }

        private SaveParticipantPayloadPreparer CreatePreparer() =>
            new SaveParticipantPayloadPreparer(
                registry,
                serializers);

        private SaveValidatedParticipantSnapshot Snapshot(
            params SavePayloadEntry[] entries) =>
            new SaveValidatedParticipantSnapshot(
                slotId,
                generationId,
                entries);

        private SavePayloadEntry Entry(
            string participantId,
            string serialized,
            int schemaVersion = 1) =>
            new SavePayloadEntry
            {
                participantId =
                    participantId,
                participantSchemaVersion =
                    schemaVersion,
                serializerId =
                    UnityJsonSaveSerializer.StableId,
                required =
                    true,
                serializedPayload =
                    serialized,
                byteProviderReference =
                    string.Empty,
                byteLength =
                    serialized == null
                        ? 0L
                        : System.Text.Encoding.UTF8
                            .GetByteCount(
                                serialized),
                checksum =
                    new string(
                        'a',
                        64),
                flags =
                    0
            };

        private TypedParticipant RegisterTyped(
            string id,
            int schemaVersion,
            params SaveParticipantId[] aliases)
        {
            TypedParticipant participant =
                new TypedParticipant(
                    Descriptor(
                        id,
                        schemaVersion,
                        aliases),
                    typeof(TestState));

            Assert.That(
                registry.Register(
                    participant)
                    .Succeeded,
                Is.True);

            return participant;
        }

        private static SaveParticipantDescriptor Descriptor(
            string id,
            int schemaVersion,
            params SaveParticipantId[] aliases) =>
            new SaveParticipantDescriptor(
                new SaveParticipantId(
                    id),
                schemaVersion,
                SaveParticipantCriticality.Required,
                SaveMissingPayloadPolicy.InitializeDefault,
                default,
                aliases);

        [Serializable]
        public sealed class TestState
        {
            public int value;
        }

        private class UntypedParticipant :
            ISaveParticipant
        {
            internal UntypedParticipant(
                SaveParticipantDescriptor descriptor)
            {
                Descriptor =
                    descriptor;
            }

            public SaveParticipantDescriptor Descriptor { get; }

            internal int CaptureCalls { get; private set; }

            internal int ApplyCalls { get; private set; }

            public SaveParticipantCaptureResult Capture()
            {
                CaptureCalls++;

                return SaveParticipantCaptureResult
                    .Failure(
                        "Preparation tests must not capture participants.");
            }

            public SaveParticipantApplyResult Apply(
                object detachedState)
            {
                ApplyCalls++;

                return SaveParticipantApplyResult
                    .Failure(
                        "Preparation tests must not apply participants.");
            }
        }

        private sealed class TypedParticipant :
            UntypedParticipant,
            ISaveTypedParticipant
        {
            internal TypedParticipant(
                SaveParticipantDescriptor descriptor,
                Type detachedStateType)
                : base(
                    descriptor)
            {
                DetachedStateType =
                    detachedStateType;
            }

            public Type DetachedStateType { get; }
        }

        private sealed class NonRuntimeSerializer :
            ISaveSerializer
        {
            public SaveSerializerId Id =>
                new SaveSerializerId(
                    "tests.nonruntime");

            public SaveSerializerResult Serialize<T>(
                T value,
                out string serialized)
            {
                serialized =
                    string.Empty;

                return SaveSerializerResult.Success(
                    "Not used.");
            }

            public SaveSerializerResult Deserialize<T>(
                string serialized,
                out T value)
            {
                value =
                    default;

                return SaveSerializerResult.Success(
                    "Not used.");
            }
        }

        private sealed class CountingRuntimeSerializer :
            IRuntimeTypeSaveSerializer
        {
            private readonly SaveSerializerId id;

            internal CountingRuntimeSerializer(
                string id)
            {
                this.id =
                    new SaveSerializerId(
                        id);
            }

            public SaveSerializerId Id =>
                id;

            internal int DeserializeCalls { get; private set; }

            public SaveSerializerResult Serialize<T>(
                T value,
                out string serialized)
            {
                serialized =
                    "{}";

                return SaveSerializerResult.Success(
                    "Test serialization succeeded.");
            }

            public SaveSerializerResult Deserialize<T>(
                string serialized,
                out T value)
            {
                value =
                    default;

                return SaveSerializerResult.Success(
                    "Generic test deserialization succeeded.");
            }

            public SaveSerializerResult Serialize(
                object value,
                Type valueType,
                out string serialized)
            {
                serialized =
                    "{}";

                return SaveSerializerResult.Success(
                    "Runtime test serialization succeeded.");
            }

            public SaveSerializerResult Deserialize(
                string serialized,
                Type valueType,
                out object value)
            {
                DeserializeCalls++;

                value =
                    new TestState
                    {
                        value =
                            77
                    };

                return SaveSerializerResult.Success(
                    "Runtime test deserialization succeeded.");
            }
        }

        private sealed class InvalidStateRuntimeSerializer :
            IRuntimeTypeSaveSerializer
        {
            public SaveSerializerId Id =>
                new SaveSerializerId(
                    "tests.invalid-state");

            public SaveSerializerResult Serialize<T>(
                T value,
                out string serialized)
            {
                serialized =
                    "{}";

                return SaveSerializerResult.Success(
                    "Test serialization succeeded.");
            }

            public SaveSerializerResult Deserialize<T>(
                string serialized,
                out T value)
            {
                value =
                    default;

                return SaveSerializerResult.Success(
                    "Generic test deserialization succeeded.");
            }

            public SaveSerializerResult Serialize(
                object value,
                Type valueType,
                out string serialized)
            {
                serialized =
                    "{}";

                return SaveSerializerResult.Success(
                    "Runtime test serialization succeeded.");
            }

            public SaveSerializerResult Deserialize(
                string serialized,
                Type valueType,
                out object value)
            {
                value =
                    null;

                return SaveSerializerResult.Success(
                    "Provider deliberately returned null.");
            }
        }
    }
}
