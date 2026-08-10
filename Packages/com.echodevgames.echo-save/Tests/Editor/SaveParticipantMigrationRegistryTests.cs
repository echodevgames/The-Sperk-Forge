using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantMigrationRegistryTests
    {
        private SaveParticipantMigrationRegistry registry;

        [SetUp]
        public void SetUp()
        {
            registry =
                new SaveParticipantMigrationRegistry();
        }

        [Test]
        public void ValidContiguousStepRegisters()
        {
            TestMigrationStep step =
                Step(
                    "tests.inventory.v1-v2",
                    "com.example.inventory",
                    1,
                    2);

            SaveParticipantMigrationRegistrationResult result =
                registry.Register(
                    step);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Registration,
                Is.Not.Null);

            Assert.That(
                result.Registration.IsActive,
                Is.True);
        }

        [Test]
        public void NonContiguousStepRejects()
        {
            TestMigrationStep step =
                Step(
                    "tests.inventory.v1-v3",
                    "com.example.inventory",
                    1,
                    3);

            SaveParticipantMigrationRegistrationResult result =
                registry.Register(
                    step);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantMigrationRegistrationStatus.InvalidStep));

            Assert.That(
                registry.Count,
                Is.Zero);
        }

        [Test]
        public void DuplicateMigrationIdRejects()
        {
            Assert.That(
                registry.Register(
                    Step(
                        "tests.shared",
                        "com.example.alpha",
                        1,
                        2))
                    .Succeeded,
                Is.True);

            SaveParticipantMigrationRegistrationResult duplicate =
                registry.Register(
                    Step(
                        "tests.shared",
                        "com.example.beta",
                        1,
                        2));

            Assert.That(
                duplicate.Status,
                Is.EqualTo(
                    SaveParticipantMigrationRegistrationStatus.DuplicateId));
        }

        [Test]
        public void DuplicateCanonicalEdgeRejects()
        {
            Assert.That(
                registry.Register(
                    Step(
                        "tests.first",
                        "com.example.inventory",
                        1,
                        2))
                    .Succeeded,
                Is.True);

            SaveParticipantMigrationRegistrationResult duplicate =
                registry.Register(
                    Step(
                        "tests.second",
                        "com.example.inventory",
                        1,
                        2));

            Assert.That(
                duplicate.Status,
                Is.EqualTo(
                    SaveParticipantMigrationRegistrationStatus.DuplicateEdge));
        }

        [Test]
        public void StaleRegistrationCannotRemoveReplacement()
        {
            SaveParticipantMigrationRegistration first =
                registry.Register(
                    Step(
                        "tests.inventory.v1-v2",
                        "com.example.inventory",
                        1,
                        2))
                    .Registration;

            registry.Clear();

            SaveParticipantMigrationRegistration replacement =
                registry.Register(
                    Step(
                        "tests.inventory.v1-v2",
                        "com.example.inventory",
                        1,
                        2))
                    .Registration;

            first.Dispose();

            Assert.That(
                replacement.IsActive,
                Is.True);

            Assert.That(
                registry.Count,
                Is.EqualTo(1));
        }

        [Test]
        public void SnapshotIsDeterministicByParticipantThenVersion()
        {
            registry.Register(
                Step(
                    "tests.zeta.v2-v3",
                    "com.example.zeta",
                    2,
                    3));

            registry.Register(
                Step(
                    "tests.alpha.v2-v3",
                    "com.example.alpha",
                    2,
                    3));

            registry.Register(
                Step(
                    "tests.alpha.v1-v2",
                    "com.example.alpha",
                    1,
                    2));

            SaveParticipantMigrationRegistrySnapshot snapshot =
                registry.GetSnapshot();

            Assert.That(
                snapshot.Count,
                Is.EqualTo(3));

            Assert.That(
                snapshot.Descriptors[0]
                    .ParticipantId.Value,
                Is.EqualTo(
                    "com.example.alpha"));

            Assert.That(
                snapshot.Descriptors[0]
                    .FromSchemaVersion,
                Is.EqualTo(1));

            Assert.That(
                snapshot.Descriptors[1]
                    .FromSchemaVersion,
                Is.EqualTo(2));

            Assert.That(
                snapshot.Descriptors[2]
                    .ParticipantId.Value,
                Is.EqualTo(
                    "com.example.zeta"));
        }

        [Test]
        public void CurrentVersionPlansZeroSteps()
        {
            SaveParticipantMigrationPlanResult result =
                registry.TryBuildPlan(
                    new SaveParticipantId(
                        "com.example.inventory"),
                    2,
                    2,
                    8,
                    out SaveParticipantMigrationPlan plan);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                plan.Count,
                Is.Zero);
        }

        [Test]
        public void OneStepPlanUsesExactEdge()
        {
            registry.Register(
                Step(
                    "tests.inventory.v1-v2",
                    "com.example.inventory",
                    1,
                    2));

            SaveParticipantMigrationPlanResult result =
                registry.TryBuildPlan(
                    new SaveParticipantId(
                        "com.example.inventory"),
                    1,
                    2,
                    8,
                    out SaveParticipantMigrationPlan plan);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                plan.Count,
                Is.EqualTo(1));

            Assert.That(
                plan.Steps[0]
                    .MigrationId.Value,
                Is.EqualTo(
                    "tests.inventory.v1-v2"));
        }

        [Test]
        public void MultiStepPlanIsAscending()
        {
            registry.Register(
                Step(
                    "tests.inventory.v2-v3",
                    "com.example.inventory",
                    2,
                    3));

            registry.Register(
                Step(
                    "tests.inventory.v1-v2",
                    "com.example.inventory",
                    1,
                    2));

            SaveParticipantMigrationPlanResult result =
                registry.TryBuildPlan(
                    new SaveParticipantId(
                        "com.example.inventory"),
                    1,
                    3,
                    8,
                    out SaveParticipantMigrationPlan plan);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                plan.Steps[0]
                    .FromSchemaVersion,
                Is.EqualTo(1));

            Assert.That(
                plan.Steps[1]
                    .FromSchemaVersion,
                Is.EqualTo(2));
        }

        [Test]
        public void MissingMiddleEdgeFailsBeforeExecution()
        {
            TestMigrationStep first =
                Step(
                    "tests.inventory.v1-v2",
                    "com.example.inventory",
                    1,
                    2);

            TestMigrationStep third =
                Step(
                    "tests.inventory.v3-v4",
                    "com.example.inventory",
                    3,
                    4);

            registry.Register(
                first);

            registry.Register(
                third);

            SaveParticipantMigrationPlanResult result =
                registry.TryBuildPlan(
                    new SaveParticipantId(
                        "com.example.inventory"),
                    1,
                    4,
                    8,
                    out SaveParticipantMigrationPlan plan);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantMigrationPlanStatus.MissingEdge));

            Assert.That(
                plan,
                Is.Null);

            Assert.That(
                first.Calls,
                Is.Zero);

            Assert.That(
                third.Calls,
                Is.Zero);
        }

        [Test]
        public void StepLimitFailsBeforeEdgeLookup()
        {
            SaveParticipantMigrationPlanResult result =
                registry.TryBuildPlan(
                    new SaveParticipantId(
                        "com.example.inventory"),
                    1,
                    5,
                    2,
                    out SaveParticipantMigrationPlan plan);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantMigrationPlanStatus.StepLimitExceeded));

            Assert.That(
                plan,
                Is.Null);

            Assert.That(
                registry.ResolveEdgeCalls,
                Is.Zero);
        }

        private static TestMigrationStep Step(
            string id,
            string participantId,
            int from,
            int to) =>
            new TestMigrationStep(
                new SaveParticipantMigrationId(
                    id),
                new SaveParticipantId(
                    participantId),
                from,
                to);

        private sealed class TestMigrationStep :
            ISaveParticipantMigrationStep
        {
            internal TestMigrationStep(
                SaveParticipantMigrationId id,
                SaveParticipantId participantId,
                int from,
                int to)
            {
                Id =
                    id;

                ParticipantId =
                    participantId;

                FromSchemaVersion =
                    from;

                ToSchemaVersion =
                    to;
            }

            public SaveParticipantMigrationId Id { get; }

            public SaveParticipantId ParticipantId { get; }

            public int FromSchemaVersion { get; }

            public int ToSchemaVersion { get; }

            internal int Calls { get; private set; }

            public SaveParticipantMigrationStepResult Migrate(
                SaveParticipantMigrationInput input)
            {
                Calls++;

                return SaveParticipantMigrationStepResult.Success(
                    ToSchemaVersion,
                    input.SerializerId.Value,
                    input.SerializedPayload);
            }
        }
    }
}
