using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantMigrationExecutorTests
    {
        private SaveParticipantMigrationRegistry registry;
        private SaveParticipantId participantId;
        private SaveParticipantId persistedId;

        [SetUp]
        public void SetUp()
        {
            registry =
                new SaveParticipantMigrationRegistry();

            participantId =
                new SaveParticipantId(
                    "com.example.inventory");

            persistedId =
                new SaveParticipantId(
                    "com.example.oldinventory");
        }

        [Test]
        public void TwoStepChainExecutesInAscendingOrder()
        {
            RecordingStep first =
                Step(
                    "tests.inventory.v1-v2",
                    1,
                    2,
                    "{\"version\":2}");

            RecordingStep second =
                Step(
                    "tests.inventory.v2-v3",
                    2,
                    3,
                    "{\"version\":3}");

            Register(
                first,
                second);

            SaveParticipantMigrationPlan plan =
                Plan(
                    1,
                    3);

            SaveParticipantMigrationExecutionResult result =
                Execute(
                    plan,
                    1,
                    "{\"version\":1}");

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                first.Calls,
                Is.EqualTo(1));

            Assert.That(
                second.Calls,
                Is.EqualTo(1));

            Assert.That(
                result.FinalSchemaVersion,
                Is.EqualTo(3));

            Assert.That(
                result.SerializedPayload,
                Is.EqualTo(
                    "{\"version\":3}"));

            Assert.That(
                result.Provenance.Length,
                Is.EqualTo(2));

            Assert.That(
                result.Provenance[0]
                    .MigrationId.Value,
                Is.EqualTo(
                    "tests.inventory.v1-v2"));

            Assert.That(
                result.Provenance[1]
                    .MigrationId.Value,
                Is.EqualTo(
                    "tests.inventory.v2-v3"));
        }

        [Test]
        public void ThrowingStepReturnsStructuredFailure()
        {
            RecordingStep step =
                Step(
                    "tests.inventory.v1-v2",
                    1,
                    2,
                    "{\"version\":2}");

            step.Throw =
                true;

            Register(
                step);

            SaveParticipantMigrationExecutionResult result =
                Execute(
                    Plan(
                        1,
                        2),
                    1,
                    "{\"version\":1}");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantMigrationExecutionStatus.StepFailed));
        }

        [Test]
        public void FailedStepReturnsStructuredFailure()
        {
            RecordingStep step =
                Step(
                    "tests.inventory.v1-v2",
                    1,
                    2,
                    "{\"version\":2}");

            step.ReturnFailure =
                true;

            Register(
                step);

            SaveParticipantMigrationExecutionResult result =
                Execute(
                    Plan(
                        1,
                        2),
                    1,
                    "{\"version\":1}");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantMigrationExecutionStatus.StepFailed));
        }

        [Test]
        public void WrongTargetVersionFailsClosed()
        {
            RecordingStep step =
                Step(
                    "tests.inventory.v1-v2",
                    1,
                    2,
                    "{\"version\":2}");

            step.ReturnTargetVersion =
                3;

            Register(
                step);

            SaveParticipantMigrationExecutionResult result =
                Execute(
                    Plan(
                        1,
                        2),
                    1,
                    "{\"version\":1}");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantMigrationExecutionStatus.InvalidOutput));
        }

        [Test]
        public void InvalidSerializerIdFailsClosed()
        {
            RecordingStep step =
                Step(
                    "tests.inventory.v1-v2",
                    1,
                    2,
                    "{\"version\":2}");

            step.ReturnSerializerId =
                "bad/id";

            Register(
                step);

            SaveParticipantMigrationExecutionResult result =
                Execute(
                    Plan(
                        1,
                        2),
                    1,
                    "{\"version\":1}");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantMigrationExecutionStatus.InvalidOutput));
        }

        [Test]
        public void NullPayloadFailsClosed()
        {
            RecordingStep step =
                Step(
                    "tests.inventory.v1-v2",
                    1,
                    2,
                    null);

            Register(
                step);

            SaveParticipantMigrationExecutionResult result =
                Execute(
                    Plan(
                        1,
                        2),
                    1,
                    "{\"version\":1}");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantMigrationExecutionStatus.InvalidOutput));
        }

        [Test]
        public void RegistryChangeAfterPlanningFailsBeforeStepCall()
        {
            RecordingStep original =
                Step(
                    "tests.inventory.v1-v2",
                    1,
                    2,
                    "{\"version\":2}");

            SaveParticipantMigrationRegistration registration =
                registry.Register(
                    original)
                    .Registration;

            SaveParticipantMigrationPlan plan =
                Plan(
                    1,
                    2);

            registration.Dispose();

            registry.Register(
                Step(
                    "tests.inventory.replacement",
                    1,
                    2,
                    "{\"version\":2}"));

            SaveParticipantMigrationExecutionResult result =
                Execute(
                    plan,
                    1,
                    "{\"version\":1}");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantMigrationExecutionStatus.RegistryChanged));

            Assert.That(
                original.Calls,
                Is.Zero);
        }

        private void Register(
            params RecordingStep[] steps)
        {
            for (int i = 0;
                 i < steps.Length;
                 i++)
            {
                Assert.That(
                    registry.Register(
                        steps[i])
                        .Succeeded,
                    Is.True);
            }
        }

        private SaveParticipantMigrationPlan Plan(
            int from,
            int to)
        {
            SaveParticipantMigrationPlanResult result =
                registry.TryBuildPlan(
                    participantId,
                    from,
                    to,
                    8,
                    out SaveParticipantMigrationPlan plan);

            Assert.That(
                result.Succeeded,
                Is.True);

            return plan;
        }

        private SaveParticipantMigrationExecutionResult Execute(
            SaveParticipantMigrationPlan plan,
            int sourceVersion,
            string serialized) =>
            SaveParticipantMigrationExecutor.Execute(
                registry,
                plan,
                new SaveParticipantMigrationInput(
                    persistedId,
                    participantId,
                    sourceVersion,
                    new SaveSerializerId(
                        UnityJsonSaveSerializer.StableId),
                    serialized,
                    true,
                    0));

        private RecordingStep Step(
            string id,
            int from,
            int to,
            string output) =>
            new RecordingStep(
                new SaveParticipantMigrationId(
                    id),
                participantId,
                from,
                to,
                output);

        private sealed class RecordingStep :
            ISaveParticipantMigrationStep
        {
            private readonly string output;

            internal RecordingStep(
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

            internal bool Throw { get; set; }

            internal bool ReturnFailure { get; set; }

            internal int? ReturnTargetVersion { get; set; }

            internal string ReturnSerializerId { get; set; }

            public SaveParticipantMigrationStepResult Migrate(
                SaveParticipantMigrationInput input)
            {
                Calls++;

                if (Throw)
                {
                    throw new InvalidOperationException(
                        "Injected migration failure.");
                }

                if (ReturnFailure)
                {
                    return SaveParticipantMigrationStepResult.Failure(
                        "TEST-MIGRATE-FAIL",
                        "Injected migration result failure.");
                }

                return SaveParticipantMigrationStepResult.Success(
                    ReturnTargetVersion ??
                        ToSchemaVersion,
                    ReturnSerializerId ??
                        input.SerializerId.Value,
                    output);
            }
        }
    }
}
