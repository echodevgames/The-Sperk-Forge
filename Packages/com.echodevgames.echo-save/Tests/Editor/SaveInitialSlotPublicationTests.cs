
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveInitialSlotPublicationTests
    {
        [Test]
        public void InitialPublicationCreatesEmptyGenerationAndHeadLast()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                SaveSlotId slotId =
                    SaveSlotId.NewId();

                SaveGenerationPublicationResult result =
                    env.CreatePublicationCoordinator()
                        .PublishInitialEmptyTransportGeneration(
                            slotId,
                            "com.example.game",
                            "1.0.0",
                            "build-a",
                            "Player Save");

                Assert.That(
                    result.Succeeded,
                    Is.True);

                Assert.That(
                    result.GenerationPublished,
                    Is.True);

                Assert.That(
                    result.HeadPublished,
                    Is.True);

                SavePayloadDocument payload =
                    env.ReadPayload(
                        slotId,
                        result.GenerationId);

                SaveManifest manifest =
                    env.ReadManifest(
                        slotId,
                        result.GenerationId);

                Assert.That(
                    payload.entries,
                    Is.Empty);

                Assert.That(
                    manifest.payloadEntries,
                    Is.Empty);

                Assert.That(
                    manifest.displayName,
                    Is.EqualTo(
                        "Player Save"));

                Assert.That(
                    env.ReadHead(
                        slotId)
                        .Succeeded,
                    Is.True);
            }
        }

        [Test]
        public void ExistingHeadIsRejectedBeforeCreateMutation()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                SaveSlotId slotId =
                    SaveSlotId.NewId();

                SaveGenerationPublicationCoordinator coordinator =
                    env.CreatePublicationCoordinator();

                Assert.That(
                    coordinator
                        .PublishInitialEmptyTransportGeneration(
                            slotId,
                            "game",
                            "1",
                            "a",
                            "First")
                        .Succeeded,
                    Is.True);

                int mutationsBefore =
                    env.Backend.MutationCount;

                SaveGenerationPublicationResult second =
                    coordinator
                        .PublishInitialEmptyTransportGeneration(
                            slotId,
                            "game",
                            "1",
                            "b",
                            "Second");

                Assert.That(
                    second.Succeeded,
                    Is.False);

                Assert.That(
                    second.Status,
                    Is.EqualTo(
                        SaveGenerationPublicationStatus
                            .ExistingHeadInvalid));

                Assert.That(
                    second.DiagnosticCode,
                    Is.EqualTo(
                        EchoSaveDiagnosticCodes
                            .SlotCreateExistingHead));

                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(
                        mutationsBefore));
            }
        }

        [TestCase(
            (int)SlotCreationFaultPoint.CandidatePayloadWrite,
            (int)SaveGenerationPublicationStatus.CandidateWriteFailed,
            false)]
        [TestCase(
            (int)SlotCreationFaultPoint.CandidateManifestWrite,
            (int)SaveGenerationPublicationStatus.CandidateWriteFailed,
            false)]
        [TestCase(
            (int)SlotCreationFaultPoint.CandidatePayloadReadCorruption,
            (int)SaveGenerationPublicationStatus.CandidateVerificationFailed,
            false)]
        [TestCase(
            (int)SlotCreationFaultPoint.GenerationPublication,
            (int)SaveGenerationPublicationStatus.GenerationPublicationFailed,
            false)]
        [TestCase(
            (int)SlotCreationFaultPoint.PublishedPayloadReadCorruption,
            (int)SaveGenerationPublicationStatus.CandidateVerificationFailed,
            true)]
        [TestCase(
            (int)SlotCreationFaultPoint.HeadPublication,
            (int)SaveGenerationPublicationStatus.HeadPublicationFailed,
            true)]
        public void InitialPublicationFailureNeverFabricatesHeadSuccess(
            int faultValue,
            int expectedStatusValue,
            bool generationPublished)
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                env.Backend.Fault =
                    (SlotCreationFaultPoint)faultValue;

                SaveSlotId slotId =
                    SaveSlotId.NewId();

                SaveGenerationPublicationResult result =
                    env.CreatePublicationCoordinator()
                        .PublishInitialEmptyTransportGeneration(
                            slotId,
                            "game",
                            "1",
                            "a",
                            "Fault");

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        (SaveGenerationPublicationStatus)
                            expectedStatusValue));

                Assert.That(
                    result.GenerationPublished,
                    Is.EqualTo(
                        generationPublished));

                Assert.That(
                    result.HeadPublished,
                    Is.False);

                Assert.That(
                    env.ReadHead(
                        slotId)
                        .Succeeded,
                    Is.False);
            }
        }
    }
}
