using System;
using System.Linq;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveManualTransactionCoordinatorTests
    {
        [Test]
        public void NullRequestFailsBeforeParticipantCapture()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                env.CreateEmptySlot();

                ManualSaveTestParticipant participant =
                    env.Participant();

                env.Register(
                    participant);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        null);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.InvalidRequest));

                Assert.That(
                    participant.CaptureCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void OversizedMetadataFailsBeforeParticipantCapture()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                env.CreateEmptySlot();

                ManualSaveTestParticipant participant =
                    env.Participant();

                env.Register(
                    participant);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request(
                            projectId: new string(
                                'x',
                                SaveManualTransactionCoordinator
                                    .MaximumMetadataTextLength + 1)));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.InvalidRequest));

                Assert.That(
                    participant.CaptureCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void NoActiveSlotFailsBeforeParticipantCapture()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                env.CreateEmptySlot(
                    select: false);

                ManualSaveTestParticipant participant =
                    env.Participant();

                env.Register(
                    participant);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.NoActiveSlot));

                Assert.That(
                    participant.CaptureCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void CatalogFailureFailsBeforeParticipantCapture()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                env.CreateEmptySlot();

                ManualSaveTestParticipant participant =
                    env.Participant();

                env.Register(
                    participant);

                env.Storage.Backend
                    .FailDiscoveryAfterHeadPublication =
                    true;

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.CatalogUnavailable));

                Assert.That(
                    participant.CaptureCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void DegradedSelectedSlotFailsBeforeParticipantCapture()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                ManualSaveTransactionTestEnvironment.CreatedSlot source =
                    env.CreateEmptySlot();

                ManualSaveTestParticipant participant =
                    env.Participant();

                env.Register(
                    participant);

                env.DeleteHead(
                    source.SlotId);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.ActiveSlotUnavailable));

                Assert.That(
                    participant.CaptureCalls,
                    Is.Zero);

                Assert.That(
                    env.Storage.Catalog.HasActiveSlot,
                    Is.False);
            }
        }

        [Test]
        public void CurrentGenerationReadFailurePublishesNothing()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                ManualSaveTransactionTestEnvironment.CreatedSlot source =
                    env.CreateEmptySlot();

                ManualSaveTestParticipant participant =
                    env.Participant();

                env.Register(
                    participant);

                int mutationsBefore =
                    env.Storage.Backend.MutationCount;

                env.Storage.Backend.Fault =
                    SlotCreationFaultPoint
                        .PublishedPayloadReadCorruption;

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.SourceReadFailed));

                Assert.That(
                    participant.CaptureCalls,
                    Is.Zero);

                Assert.That(
                    env.Storage.Backend.MutationCount,
                    Is.EqualTo(
                        mutationsBefore));

                Assert.That(
                    env.ReadHead(
                            source.SlotId)
                        .currentGenerationId,
                    Is.EqualTo(
                        source.GenerationId.Value));
            }
        }

        [Test]
        public void ParticipantCaptureFailureDoesNotAdvanceHead()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                ManualSaveTransactionTestEnvironment.CreatedSlot source =
                    env.CreateEmptySlot();

                ManualSaveTestParticipant participant =
                    env.Participant(
                        failCapture: true);

                env.Register(
                    participant);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.CaptureFailed));

                Assert.That(
                    result.GenerationPublished,
                    Is.False);

                Assert.That(
                    result.HeadPublished,
                    Is.False);

                Assert.That(
                    env.ReadHead(
                            source.SlotId)
                        .currentGenerationId,
                    Is.EqualTo(
                        source.GenerationId.Value));
            }
        }

        [Test]
        public void SuccessfulSaveAdvancesHeadPreservesDisplayAndSelection()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                const string displayName =
                    "Chronicle Slot Alpha";

                ManualSaveTransactionTestEnvironment.CreatedSlot source =
                    env.CreateEmptySlot(
                        displayName);

                ManualSaveTestParticipant participant =
                    env.Participant(
                        value: 313);

                env.Register(
                    participant);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request());

                Assert.That(
                    result.Succeeded,
                    Is.True);

                Assert.That(
                    result.SourceGenerationId,
                    Is.EqualTo(
                        source.GenerationId));

                Assert.That(
                    result.PublishedGenerationId,
                    Is.Not.EqualTo(
                        source.GenerationId));

                Assert.That(
                    result.GenerationPublished,
                    Is.True);

                Assert.That(
                    result.HeadPublished,
                    Is.True);

                Assert.That(
                    result.CatalogReconciled,
                    Is.True);

                Assert.That(
                    result.FreshParticipantCount,
                    Is.EqualTo(1));

                Assert.That(
                    result.PreservedUnknownCount,
                    Is.Zero);

                SaveHeadPointer head =
                    env.ReadHead(
                        source.SlotId);

                Assert.That(
                    head.currentGenerationId,
                    Is.EqualTo(
                        result.PublishedGenerationId.Value));

                Assert.That(
                    head.previousGenerationId,
                    Is.EqualTo(
                        source.GenerationId.Value));

                Assert.That(
                    result.ReconciledEntry.DisplayName,
                    Is.EqualTo(
                        displayName));

                Assert.That(
                    env.Storage.Catalog.HasActiveSlot,
                    Is.True);

                Assert.That(
                    env.Storage.Catalog.ActiveSlotId,
                    Is.EqualTo(
                        source.SlotId));

                Assert.That(
                    participant.ApplyCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void OpaqueUnknownPayloadSurvivesByteForByte()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                const string unknownId =
                    "com.example.future";

                const string unknownJson =
                    "{\"future\":\"opaque-313\"}";

                ManualSaveTransactionTestEnvironment.CreatedSlot source =
                    env.InstallParticipantSource(
                        unknownId,
                        unknownJson);

                SavePayloadDocument sourcePayload =
                    env.Storage.ReadPayload(
                        source.SlotId,
                        source.GenerationId);

                SavePayloadEntry sourceUnknown =
                    sourcePayload.entries
                        .Single(
                            entry =>
                                entry.participantId ==
                                unknownId);

                env.Register(
                    env.Participant(
                        "com.example.inventory",
                        55));

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request());

                Assert.That(
                    result.Succeeded,
                    Is.True);

                Assert.That(
                    result.FreshParticipantCount,
                    Is.EqualTo(1));

                Assert.That(
                    result.PreservedUnknownCount,
                    Is.EqualTo(1));

                SavePayloadDocument publishedPayload =
                    env.Storage.ReadPayload(
                        source.SlotId,
                        result.PublishedGenerationId);

                SavePayloadEntry preserved =
                    publishedPayload.entries
                        .Single(
                            entry =>
                                entry.participantId ==
                                unknownId);

                Assert.That(
                    preserved.serializedPayload,
                    Is.EqualTo(
                        sourceUnknown.serializedPayload));

                Assert.That(
                    preserved.byteLength,
                    Is.EqualTo(
                        sourceUnknown.byteLength));

                Assert.That(
                    preserved.checksum,
                    Is.EqualTo(
                        sourceUnknown.checksum));

                Assert.That(
                    preserved.serializerId,
                    Is.EqualTo(
                        sourceUnknown.serializerId));
            }
        }

        [Test]
        public void OwnershipCollisionAfterReadBlocksPublication()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                const string futureId =
                    "com.example.future";

                ManualSaveTransactionTestEnvironment.CreatedSlot source =
                    env.InstallParticipantSource(
                        futureId,
                        "{\"future\":true}");

                ManualSaveTestParticipant futureOwner =
                    env.Participant(
                        futureId,
                        99);

                ManualSaveTestParticipant trigger =
                    env.Participant(
                        "com.example.inventory",
                        1,
                        onCapture: () =>
                            env.Register(
                                futureOwner));

                env.Register(
                    trigger);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.CarryForwardFailed));

                Assert.That(
                    result.FailingParticipantId.Value,
                    Is.EqualTo(
                        futureId));

                Assert.That(
                    result.CurrentOwnerId.Value,
                    Is.EqualTo(
                        futureId));

                Assert.That(
                    result.HeadPublished,
                    Is.False);

                Assert.That(
                    env.ReadHead(
                            source.SlotId)
                        .currentGenerationId,
                    Is.EqualTo(
                        source.GenerationId.Value));
            }
        }

        [Test]
        public void SourceAdvanceDuringCaptureIsRejectedAsStale()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                ManualSaveTransactionTestEnvironment.CreatedSlot source =
                    env.CreateEmptySlot(
                        "Stale Source Slot");

                SaveGenerationId interveningGeneration =
                    default;

                ManualSaveTestParticipant participant =
                    env.Participant(
                        onCapture: () =>
                        {
                            SaveGenerationPublicationResult
                                intervening =
                                    env.PublishInterveningGeneration(
                                        source.SlotId,
                                        source.DisplayName);

                            Assert.That(
                                intervening.Succeeded,
                                Is.True);

                            interveningGeneration =
                                intervening.GenerationId;
                        });

                env.Register(
                    participant);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.StaleSource));

                Assert.That(
                    result.GenerationPublished,
                    Is.False);

                Assert.That(
                    result.HeadPublished,
                    Is.False);

                Assert.That(
                    env.ReadHead(
                            source.SlotId)
                        .currentGenerationId,
                    Is.EqualTo(
                        interveningGeneration.Value));
            }
        }

        [Test]
        public void FinalVerificationFailureReportsPublishedGenerationWithoutHead()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                ManualSaveTransactionTestEnvironment.CreatedSlot source =
                    env.CreateEmptySlot();

                ManualSaveTestParticipant participant =
                    env.Participant(
                        onCapture: () =>
                            env.Storage.Backend.Fault =
                                SlotCreationFaultPoint
                                    .PublishedPayloadReadCorruption);

                env.Register(
                    participant);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.PublicationFailed));

                Assert.That(
                    result.GenerationPublished,
                    Is.True);

                Assert.That(
                    result.HeadPublished,
                    Is.False);

                Assert.That(
                    env.ReadHead(
                            source.SlotId)
                        .currentGenerationId,
                    Is.EqualTo(
                        source.GenerationId.Value));
            }
        }

        [Test]
        public void HeadPublicationFailureNeverFabricatesHeadSuccess()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                ManualSaveTransactionTestEnvironment.CreatedSlot source =
                    env.CreateEmptySlot();

                ManualSaveTestParticipant participant =
                    env.Participant(
                        onCapture: () =>
                            env.Storage.Backend.Fault =
                                SlotCreationFaultPoint
                                    .HeadPublication);

                env.Register(
                    participant);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.PublicationFailed));

                Assert.That(
                    result.GenerationPublished,
                    Is.True);

                Assert.That(
                    result.HeadPublished,
                    Is.False);

                Assert.That(
                    env.ReadHead(
                            source.SlotId)
                        .currentGenerationId,
                    Is.EqualTo(
                        source.GenerationId.Value));
            }
        }

        [Test]
        public void CatalogFailureAfterHeadPreservesDurableTruth()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                ManualSaveTransactionTestEnvironment.CreatedSlot source =
                    env.CreateEmptySlot();

                ManualSaveTestParticipant participant =
                    env.Participant(
                        onCapture: () =>
                            env.Storage.Backend
                                .FailDiscoveryAfterHeadPublication =
                                true);

                env.Register(
                    participant);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus
                            .PublishedCatalogReconciliationFailed));

                Assert.That(
                    result.GenerationPublished,
                    Is.True);

                Assert.That(
                    result.HeadPublished,
                    Is.True);

                Assert.That(
                    result.CatalogReconciled,
                    Is.False);

                Assert.That(
                    env.ReadHead(
                            source.SlotId)
                        .currentGenerationId,
                    Is.EqualTo(
                        result.PublishedGenerationId.Value));
            }
        }

    }
}
