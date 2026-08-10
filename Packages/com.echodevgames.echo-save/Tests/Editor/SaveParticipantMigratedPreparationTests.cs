using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantMigratedPreparationTests
    {
        private SaveParticipantRegistry participants;
        private SaveSerializerRegistry serializers;
        private SaveParticipantMigrationRegistry migrations;
        private SaveSlotId slotId;
        private SaveGenerationId generationId;

        [SetUp]
        public void SetUp()
        {
            participants =
                new SaveParticipantRegistry();

            serializers =
                new SaveSerializerRegistry();

            migrations =
                new SaveParticipantMigrationRegistry();

            slotId =
                SaveSlotId.NewId();

            generationId =
                SaveGenerationId.NewId();
        }

        [Test]
        public void OlderPayloadMigratesThenUsesCurrentDtoDeserializer()
        {
            TypedParticipant participant =
                RegisterParticipant(
                    "com.example.inventory",
                    2);

            RecordingMigration step =
                RegisterMigration(
                    "tests.inventory.v1-v2",
                    "com.example.inventory",
                    1,
                    2,
                    "{\"value\":42}");

            SaveParticipantPreparationResult result =
                Preparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                1,
                                "{\"oldValue\":42}")));

            Assert.That(
                result.Succeeded,
                Is.True);

            SavePreparedParticipantEntry prepared =
                result.Batch.Entries[0];

            Assert.That(
                prepared.StoredParticipantSchemaVersion,
                Is.EqualTo(1));

            Assert.That(
                prepared.ParticipantSchemaVersion,
                Is.EqualTo(2));

            Assert.That(
                prepared.MigrationStepCount,
                Is.EqualTo(1));

            Assert.That(
                prepared.MigrationProvenance[0]
                    .MigrationId.Value,
                Is.EqualTo(
                    "tests.inventory.v1-v2"));

            Assert.That(
                ((TestState)prepared.DetachedState)
                    .value,
                Is.EqualTo(42));

            Assert.That(
                step.Calls,
                Is.EqualTo(1));

            Assert.That(
                participant.CaptureCalls,
                Is.Zero);

            Assert.That(
                participant.ApplyCalls,
                Is.Zero);
        }

        [Test]
        public void PersistedAliasUsesCanonicalMigrationRoute()
        {
            RegisterParticipant(
                "com.example.inventory",
                2,
                new SaveParticipantId(
                    "com.example.oldinventory"));

            RecordingMigration step =
                RegisterMigration(
                    "tests.inventory.v1-v2",
                    "com.example.inventory",
                    1,
                    2,
                    "{\"value\":7}");

            SaveParticipantPreparationResult result =
                Preparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.oldinventory",
                                1,
                                "{\"oldValue\":7}")));

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                step.Calls,
                Is.EqualTo(1));

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
        public void MissingMigrationEdgeReturnsNoPreparedBatch()
        {
            RegisterParticipant(
                "com.example.inventory",
                3);

            RegisterMigration(
                "tests.inventory.v1-v2",
                "com.example.inventory",
                1,
                2,
                "{\"value\":2}");

            SaveParticipantPreparationResult result =
                Preparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                1,
                                "{\"value\":1}")));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantPreparationStatus
                        .MigrationChainUnavailable));

            Assert.That(
                result.Batch,
                Is.Null);
        }

        [Test]
        public void StepFailureReturnsNoPreparedBatch()
        {
            RegisterParticipant(
                "com.example.inventory",
                2);

            RecordingMigration step =
                RegisterMigration(
                    "tests.inventory.v1-v2",
                    "com.example.inventory",
                    1,
                    2,
                    "{\"value\":2}");

            step.ReturnFailure =
                true;

            SaveParticipantPreparationResult result =
                Preparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                1,
                                "{\"value\":1}")));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantPreparationStatus
                        .MigrationFailed));

            Assert.That(
                result.Batch,
                Is.Null);
        }

        [Test]
        public void UnknownPayloadDoesNotTouchMigrationRegistry()
        {
            RegisterMigration(
                "tests.future.v1-v2",
                "com.example.future",
                1,
                2,
                "{\"value\":2}");

            int before =
                migrations.ResolveEdgeCalls;

            SaveParticipantPreparationResult result =
                Preparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.future",
                                1,
                                "{\"value\":1}")));

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.PreparedCount,
                Is.Zero);

            Assert.That(
                migrations.ResolveEdgeCalls,
                Is.EqualTo(
                    before));
        }

        [Test]
        public void MixedCurrentAndMigratedParticipantsRemainAllOrNothing()
        {
            RegisterParticipant(
                "com.example.alpha",
                2);

            RegisterParticipant(
                "com.example.beta",
                1);

            RecordingMigration step =
                RegisterMigration(
                    "tests.alpha.v1-v2",
                    "com.example.alpha",
                    1,
                    2,
                    "{bad-json");

            SaveParticipantPreparationResult result =
                Preparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.alpha",
                                1,
                                "{\"value\":1}"),
                            Entry(
                                "com.example.beta",
                                1,
                                "{\"value\":2}")));

            Assert.That(
                result.Succeeded,
                Is.False);

            Assert.That(
                result.Batch,
                Is.Null);

            Assert.That(
                result.PreparedCount,
                Is.Zero);

            Assert.That(
                step.Calls,
                Is.EqualTo(1));
        }

        [Test]
        public void MultiStepMigrationProvenanceIsOrdered()
        {
            RegisterParticipant(
                "com.example.inventory",
                3);

            RegisterMigration(
                "tests.inventory.v2-v3",
                "com.example.inventory",
                2,
                3,
                "{\"value\":3}");

            RegisterMigration(
                "tests.inventory.v1-v2",
                "com.example.inventory",
                1,
                2,
                "{\"value\":2}");

            SaveParticipantPreparationResult result =
                Preparer()
                    .Prepare(
                        Snapshot(
                            Entry(
                                "com.example.inventory",
                                1,
                                "{\"value\":1}")));

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Batch.Entries[0]
                    .MigrationStepCount,
                Is.EqualTo(2));

            Assert.That(
                result.Batch.Entries[0]
                    .MigrationProvenance[0]
                    .FromSchemaVersion,
                Is.EqualTo(1));

            Assert.That(
                result.Batch.Entries[0]
                    .MigrationProvenance[1]
                    .FromSchemaVersion,
                Is.EqualTo(2));
        }

        private SaveParticipantPayloadPreparer Preparer() =>
            new SaveParticipantPayloadPreparer(
                participants,
                serializers,
                migrations,
                8);

        private SaveValidatedParticipantSnapshot Snapshot(
            params SavePayloadEntry[] entries) =>
            new SaveValidatedParticipantSnapshot(
                slotId,
                generationId,
                entries);

        private static SavePayloadEntry Entry(
            string participantId,
            int schemaVersion,
            string serialized) =>
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
                        ? 0
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

        private TypedParticipant RegisterParticipant(
            string id,
            int currentSchemaVersion,
            params SaveParticipantId[] aliases)
        {
            TypedParticipant participant =
                new TypedParticipant(
                    new SaveParticipantDescriptor(
                        new SaveParticipantId(
                            id),
                        currentSchemaVersion,
                        SaveParticipantCriticality.Required,
                        SaveMissingPayloadPolicy.InitializeDefault,
                        default,
                        aliases));

            Assert.That(
                participants.Register(
                    participant)
                    .Succeeded,
                Is.True);

            return participant;
        }

        private RecordingMigration RegisterMigration(
            string migrationId,
            string participantId,
            int from,
            int to,
            string output)
        {
            RecordingMigration step =
                new RecordingMigration(
                    new SaveParticipantMigrationId(
                        migrationId),
                    new SaveParticipantId(
                        participantId),
                    from,
                    to,
                    output);

            Assert.That(
                migrations.Register(
                    step)
                    .Succeeded,
                Is.True);

            return step;
        }

        [Serializable]
        public sealed class TestState
        {
            public int value;
        }

        private sealed class TypedParticipant :
            ISaveTypedParticipant
        {
            internal TypedParticipant(
                SaveParticipantDescriptor descriptor)
            {
                Descriptor =
                    descriptor;
            }

            public SaveParticipantDescriptor Descriptor { get; }

            public Type DetachedStateType =>
                typeof(TestState);

            internal int CaptureCalls { get; private set; }

            internal int ApplyCalls { get; private set; }

            public SaveParticipantCaptureResult Capture()
            {
                CaptureCalls++;

                return SaveParticipantCaptureResult.Failure(
                    "Migration-preparation tests must not capture.");
            }

            public SaveParticipantApplyResult Apply(
                object detachedState)
            {
                ApplyCalls++;

                return SaveParticipantApplyResult.Failure(
                    "Migration-preparation tests must not apply.");
            }
        }

        private sealed class RecordingMigration :
            ISaveParticipantMigrationStep
        {
            private readonly string output;

            internal RecordingMigration(
                SaveParticipantMigrationId id,
                SaveParticipantId participantId,
                int from,
                int to,
                string output)
            {
                Id =
                    id;

                ParticipantId =
                    participantId;

                FromSchemaVersion =
                    from;

                ToSchemaVersion =
                    to;

                this.output =
                    output;
            }

            public SaveParticipantMigrationId Id { get; }

            public SaveParticipantId ParticipantId { get; }

            public int FromSchemaVersion { get; }

            public int ToSchemaVersion { get; }

            internal int Calls { get; private set; }

            internal bool ReturnFailure { get; set; }

            public SaveParticipantMigrationStepResult Migrate(
                SaveParticipantMigrationInput input)
            {
                Calls++;

                if (ReturnFailure)
                {
                    return SaveParticipantMigrationStepResult.Failure(
                        "TEST-MIGRATION-FAIL",
                        "Injected migration failure.");
                }

                return SaveParticipantMigrationStepResult.Success(
                    ToSchemaVersion,
                    input.SerializerId.Value,
                    output);
            }
        }
    }
}
