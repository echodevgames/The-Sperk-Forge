
using System.Collections.Generic;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveSlotMutationCoordinatorTests
    {
        [Test]
        public void RenameChangesDisplayNameWhileSlotIdentityAndSourceBytesRemainStable()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource("Original");

                byte[] payloadBefore =
                    env.ReadRawPayload(
                        source.SlotId,
                        source.GenerationId);

                byte[] manifestBefore =
                    env.ReadRawManifest(
                        source.SlotId,
                        source.GenerationId);

                SaveSlotRenameResult result =
                    env.Coordinator().Rename(
                        new SaveSlotRenameRequest(
                            source.SlotId,
                            "Renamed"));

                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.HeadPublished, Is.True);
                Assert.That(result.SlotId, Is.EqualTo(source.SlotId));
                Assert.That(
                    result.PublishedGenerationId,
                    Is.Not.EqualTo(source.GenerationId));

                SaveHeadPointer head =
                    env.ReadHead(source.SlotId);

                Assert.That(head.slotId, Is.EqualTo(source.SlotId.Value));
                Assert.That(
                    head.currentGenerationId,
                    Is.EqualTo(result.PublishedGenerationId.Value));

                SaveManifest renamed =
                    env.ReadManifest(
                        source.SlotId,
                        result.PublishedGenerationId);

                Assert.That(renamed.displayName, Is.EqualTo("Renamed"));

                CollectionAssert.AreEqual(
                    payloadBefore,
                    env.ReadRawPayload(
                        source.SlotId,
                        source.GenerationId));

                CollectionAssert.AreEqual(
                    manifestBefore,
                    env.ReadRawManifest(
                        source.SlotId,
                        source.GenerationId));

                SavePayloadDocument oldPayload =
                    env.ReadPayload(
                        source.SlotId,
                        source.GenerationId);

                SavePayloadDocument newPayload =
                    env.ReadPayload(
                        source.SlotId,
                        result.PublishedGenerationId);

                Assert.That(
                    newPayload.entries[0].serializedPayload,
                    Is.EqualTo(
                        oldPayload.entries[0].serializedPayload));
            }
        }

        [Test]
        public void RenamePreservesActiveSlotSelection()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource();

                Assert.That(
                    env.Catalog.SelectActiveSlot(
                        source.SlotId).Succeeded,
                    Is.True);

                SaveSlotRenameResult result =
                    env.Coordinator().Rename(
                        new SaveSlotRenameRequest(
                            source.SlotId,
                            "Still Active"));

                Assert.That(result.HeadPublished, Is.True);
                Assert.That(env.Catalog.HasActiveSlot, Is.True);
                Assert.That(
                    env.Catalog.ActiveSlotId,
                    Is.EqualTo(source.SlotId));
            }
        }

        [Test]
        public void RenameToSameDisplayNameIsNoChangeWithoutMutation()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource("Same");

                int mutationsBefore =
                    env.Backend.MutationCount;

                SaveSlotRenameResult result =
                    env.Coordinator().Rename(
                        new SaveSlotRenameRequest(
                            source.SlotId,
                            "Same"));

                Assert.That(
                    result.Status,
                    Is.EqualTo(SaveSlotRenameStatus.NoChange));
                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.HeadPublished, Is.False);
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(mutationsBefore));
            }
        }

        [Test]
        public void RenameMissingSlotRejectsBeforeMutation()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                int mutationsBefore =
                    env.Backend.MutationCount;

                SaveSlotRenameResult result =
                    env.Coordinator().Rename(
                        new SaveSlotRenameRequest(
                            SaveSlotId.NewId(),
                            "Missing"));

                Assert.That(
                    result.Status,
                    Is.EqualTo(SaveSlotRenameStatus.SlotNotFound));
                Assert.That(result.HeadPublished, Is.False);
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(mutationsBefore));
            }
        }

        [Test]
        public void RenameDegradedSourceRejectsBeforeMutation()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource();

                env.CorruptCurrentPayload(source.SlotId);
                Assert.That(env.Catalog.Refresh().Succeeded, Is.True);

                int mutationsBefore =
                    env.Backend.MutationCount;

                SaveSlotRenameResult result =
                    env.Coordinator().Rename(
                        new SaveSlotRenameRequest(
                            source.SlotId,
                            "Blocked"));

                Assert.That(
                    result.Status,
                    Is.EqualTo(SaveSlotRenameStatus.SourceInvalid));
                Assert.That(result.HeadPublished, Is.False);
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(mutationsBefore));
            }
        }

        [Test]
        public void RenameStaleSourceRevalidationRejectsBeforePublication()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource();

                int mutationsBefore =
                    env.Backend.MutationCount;

                SaveSlotRenameResult result =
                    env.Coordinator(
                            sourceReader:
                                new StaleSlotMutationSourceReader(
                                    env.SourceReader))
                        .Rename(
                            new SaveSlotRenameRequest(
                                source.SlotId,
                                "Stale"));

                Assert.That(
                    result.Status,
                    Is.EqualTo(SaveSlotRenameStatus.SourceStale));
                Assert.That(result.HeadPublished, Is.False);
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(mutationsBefore));
            }
        }

        [Test]
        public void RepeatedRenameRespectsExistingRetentionBound()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource();

                SaveSlotMutationCoordinator coordinator =
                    env.Coordinator();

                for (int i = 0; i < 8; i++)
                {
                    SaveSlotRenameResult result =
                        coordinator.Rename(
                            new SaveSlotRenameRequest(
                                source.SlotId,
                                "Rename " + i));

                    Assert.That(result.HeadPublished, Is.True);
                }

                Assert.That(
                    env.CountCommittedGenerations(source.SlotId),
                    Is.LessThanOrEqualTo(
                        SaveRetentionPolicy.DefaultTotalGenerations));
            }
        }

        [Test]
        public void RenameCatalogFailureAfterHeadPreservesCommittedTruth()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource();

                env.Backend.FailCatalogDiscoveryAfterNextHead();

                SaveSlotRenameResult result =
                    env.Coordinator().Rename(
                        new SaveSlotRenameRequest(
                            source.SlotId,
                            "Committed"));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveSlotRenameStatus
                            .PublishedCatalogReconciliationFailed));
                Assert.That(result.HeadPublished, Is.True);
                Assert.That(result.RenameCommitted, Is.True);
                Assert.That(result.CatalogReconciled, Is.False);
                Assert.That(
                    env.ReadHead(source.SlotId).currentGenerationId,
                    Is.EqualTo(result.PublishedGenerationId.Value));
            }
        }

        [Test]
        public void DuplicateCreatesNewIdentityWithEquivalentStateAndPreservesSourceBytes()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource("Copy Me");

                byte[] payloadBefore =
                    env.ReadRawPayload(
                        source.SlotId,
                        source.GenerationId);

                byte[] manifestBefore =
                    env.ReadRawManifest(
                        source.SlotId,
                        source.GenerationId);

                SaveSlotDuplicateResult result =
                    env.Coordinator().Duplicate(
                        new SaveSlotDuplicateRequest(
                            source.SlotId));

                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.HeadPublished, Is.True);
                Assert.That(
                    result.DuplicateSlotId,
                    Is.Not.EqualTo(source.SlotId));
                Assert.That(
                    result.DuplicateGenerationId,
                    Is.Not.EqualTo(source.GenerationId));

                SaveManifest destinationManifest =
                    env.ReadManifest(
                        result.DuplicateSlotId,
                        result.DuplicateGenerationId);

                Assert.That(
                    destinationManifest.displayName,
                    Is.EqualTo("Copy Me"));

                SavePayloadDocument sourcePayload =
                    env.ReadPayload(
                        source.SlotId,
                        source.GenerationId);

                SavePayloadDocument destinationPayload =
                    env.ReadPayload(
                        result.DuplicateSlotId,
                        result.DuplicateGenerationId);

                Assert.That(
                    destinationPayload.entries[0].serializedPayload,
                    Is.EqualTo(
                        sourcePayload.entries[0].serializedPayload));
                Assert.That(
                    destinationPayload.entries[0].checksum,
                    Is.EqualTo(
                        sourcePayload.entries[0].checksum));

                CollectionAssert.AreEqual(
                    payloadBefore,
                    env.ReadRawPayload(
                        source.SlotId,
                        source.GenerationId));

                CollectionAssert.AreEqual(
                    manifestBefore,
                    env.ReadRawManifest(
                        source.SlotId,
                        source.GenerationId));
            }
        }

        [Test]
        public void DuplicateDoesNotAutoSelectNewSlot()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource();

                Assert.That(
                    env.Catalog.SelectActiveSlot(
                        source.SlotId).Succeeded,
                    Is.True);

                SaveSlotDuplicateResult result =
                    env.Coordinator().Duplicate(
                        new SaveSlotDuplicateRequest(
                            source.SlotId));

                Assert.That(result.Succeeded, Is.True);
                Assert.That(env.Catalog.HasActiveSlot, Is.True);
                Assert.That(
                    env.Catalog.ActiveSlotId,
                    Is.EqualTo(source.SlotId));
                Assert.That(
                    result.DuplicateSlotId,
                    Is.Not.EqualTo(env.Catalog.ActiveSlotId));
            }
        }

        [Test]
        public void DuplicateCapacityReachedPerformsNoDestinationMutation()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource();

                int mutationsBefore =
                    env.Backend.MutationCount;

                SaveSlotDuplicateResult result =
                    env.Coordinator(capacity: 1).Duplicate(
                        new SaveSlotDuplicateRequest(
                            source.SlotId));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveSlotDuplicateStatus.CapacityReached));
                Assert.That(result.HeadPublished, Is.False);
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(mutationsBefore));
            }
        }

        [Test]
        public void DuplicateStaleSourceRevalidationRejectsBeforeDestinationPublication()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource();

                int mutationsBefore =
                    env.Backend.MutationCount;

                SaveSlotDuplicateResult result =
                    env.Coordinator(
                            sourceReader:
                                new StaleSlotMutationSourceReader(
                                    env.SourceReader))
                        .Duplicate(
                            new SaveSlotDuplicateRequest(
                                source.SlotId));

                Assert.That(
                    result.Status,
                    Is.EqualTo(SaveSlotDuplicateStatus.SourceStale));
                Assert.That(result.HeadPublished, Is.False);
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(mutationsBefore));
            }
        }

        [Test]
        public void DuplicateCollisionRetrySkipsExistingCanonicalIdentity()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource();

                SaveSlotId fresh =
                    SaveSlotId.NewId();

                Queue<SaveSlotId> ids =
                    new Queue<SaveSlotId>(
                        new[]
                        {
                            source.SlotId,
                            fresh
                        });

                SaveSlotDuplicateResult result =
                    env.Coordinator(
                            maxIdAttempts: 2,
                            slotIdFactory: ids.Dequeue)
                        .Duplicate(
                            new SaveSlotDuplicateRequest(
                                source.SlotId));

                Assert.That(result.Succeeded, Is.True);
                Assert.That(
                    result.DuplicateSlotId,
                    Is.EqualTo(fresh));
            }
        }

        [Test]
        public void DuplicateCatalogFailureAfterHeadPreservesCommittedTruth()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource();

                env.Backend.FailCatalogDiscoveryAfterNextHead();

                SaveSlotDuplicateResult result =
                    env.Coordinator().Duplicate(
                        new SaveSlotDuplicateRequest(
                            source.SlotId));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveSlotDuplicateStatus
                            .PublishedCatalogReconciliationFailed));
                Assert.That(result.HeadPublished, Is.True);
                Assert.That(result.DuplicateCommitted, Is.True);
                Assert.That(result.CatalogReconciled, Is.False);
                Assert.That(
                    env.ReadHead(result.DuplicateSlotId).currentGenerationId,
                    Is.EqualTo(result.DuplicateGenerationId.Value));
            }
        }

        [Test]
        public void DuplicateDegradedSourceRejectsBeforeMutation()
        {
            using (SaveSlotMutationTestEnvironment env =
                new SaveSlotMutationTestEnvironment())
            {
                SaveSlotMutationTestEnvironment.CreatedSource source =
                    env.CreateSource();

                env.CorruptCurrentPayload(source.SlotId);
                Assert.That(env.Catalog.Refresh().Succeeded, Is.True);

                int mutationsBefore =
                    env.Backend.MutationCount;

                SaveSlotDuplicateResult result =
                    env.Coordinator().Duplicate(
                        new SaveSlotDuplicateRequest(
                            source.SlotId));

                Assert.That(
                    result.Status,
                    Is.EqualTo(SaveSlotDuplicateStatus.SourceInvalid));
                Assert.That(result.HeadPublished, Is.False);
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(mutationsBefore));
            }
        }
    }
}
