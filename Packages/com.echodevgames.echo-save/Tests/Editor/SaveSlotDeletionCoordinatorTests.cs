
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveSlotDeletionCoordinatorTests
    {
        [Test]
        public void ConfirmWithoutPlanPerformsNoMutation()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                int before = env.Backend.MutationCount;

                SaveSlotDeleteResult result =
                    env.Coordinator().Confirm(null);

                Assert.That(
                    result.Status,
                    Is.EqualTo(SaveSlotDeleteStatus.InvalidPlan));
                Assert.That(result.DeleteCommitted, Is.False);
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(before));
            }
        }

        [Test]
        public void PrepareDeletePerformsZeroDurableMutation()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source = env.CreateSource();
                int before = env.Backend.MutationCount;

                SaveDeletionPlan plan =
                    env.Coordinator().Prepare(
                        source.SlotId);

                Assert.That(plan.Succeeded, Is.True);
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(before));
                Assert.That(
                    env.LiveSlotDirectoryExists(source.SlotId),
                    Is.True);
            }
        }

        [Test]
        public void PreparedPlanBindsSlotGenerationMetadataAndExpiry()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source =
                    env.CreateSource("Prepared Delete");

                Assert.That(
                    env.Catalog.SelectActiveSlot(
                        source.SlotId).Succeeded,
                    Is.True);

                SaveDeletionPlan plan =
                    env.Coordinator().Prepare(
                        source.SlotId);

                Assert.That(plan.Succeeded, Is.True);
                Assert.That(plan.SlotId, Is.EqualTo(source.SlotId));
                Assert.That(
                    plan.CurrentGenerationId,
                    Is.EqualTo(source.GenerationId));
                Assert.That(
                    plan.DisplayName,
                    Is.EqualTo("Prepared Delete"));
                Assert.That(plan.WasActiveSlot, Is.True);
                Assert.That(
                    plan.ExpiresUtc,
                    Is.GreaterThan(plan.IssuedUtc));
                Assert.That(plan.PlanId.Length, Is.EqualTo(32));
            }
        }

        [Test]
        public void MissingSlotCannotProduceReadyDeletionPlan()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                int before = env.Backend.MutationCount;

                SaveDeletionPlan plan =
                    env.Coordinator().Prepare(
                        SaveSlotId.NewId());

                Assert.That(
                    plan.Status,
                    Is.EqualTo(SaveDeletionPlanStatus.SlotNotFound));
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(before));
            }
        }

        [Test]
        public void ExpiredDeletionPlanRejectsWithoutMutation()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source = env.CreateSource();

                SaveSlotDeletionCoordinator coordinator =
                    env.Coordinator(
                        planLifetime: TimeSpan.FromSeconds(5));

                SaveDeletionPlan plan =
                    coordinator.Prepare(source.SlotId);

                env.Clock =
                    env.Clock.AddSeconds(6);

                int before = env.Backend.MutationCount;

                SaveSlotDeleteResult result =
                    coordinator.Confirm(plan);

                Assert.That(
                    result.Status,
                    Is.EqualTo(SaveSlotDeleteStatus.Expired));
                Assert.That(result.DeleteCommitted, Is.False);
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(before));
                Assert.That(
                    env.LiveSlotDirectoryExists(source.SlotId),
                    Is.True);
            }
        }

        [Test]
        public void ForeignSessionDeletionPlanRejectsWithoutMutation()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source = env.CreateSource();

                SaveDeletionPlan plan =
                    env.Coordinator(
                            sessionId: "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")
                        .Prepare(source.SlotId);

                int before = env.Backend.MutationCount;

                SaveSlotDeleteResult result =
                    env.Coordinator(
                            sessionId: "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb")
                        .Confirm(plan);

                Assert.That(
                    result.Status,
                    Is.EqualTo(SaveSlotDeleteStatus.ForeignSession));
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(before));
            }
        }

        [Test]
        public void StalePreparedSourceRejectsBeforeTrashMutation()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source = env.CreateSource();

                SaveDeletionSourceReader reader =
                    new SaveDeletionSourceReader(
                        env.Backend,
                        env.Serializer,
                        env.Integrity);

                SaveSlotDeletionCoordinator coordinator =
                    env.Coordinator(
                        sourceReader:
                            new StaleDeletionSourceReader(reader));

                SaveDeletionPlan plan =
                    coordinator.Prepare(source.SlotId);

                int before = env.Backend.MutationCount;

                SaveSlotDeleteResult result =
                    coordinator.Confirm(plan);

                Assert.That(
                    result.Status,
                    Is.EqualTo(SaveSlotDeleteStatus.SourceStale));
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(before));
                Assert.That(
                    env.LiveSlotDirectoryExists(source.SlotId),
                    Is.True);
            }
        }

        [Test]
        public void ConfirmDeleteMovesCompleteSlotIntoRecoverableTrash()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source = env.CreateSource();

                SaveSlotDeletionCoordinator coordinator =
                    env.Coordinator();

                SaveDeletionPlan plan =
                    coordinator.Prepare(source.SlotId);

                SaveSlotDeleteResult result =
                    coordinator.Confirm(plan);

                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.DeleteCommitted, Is.True);
                Assert.That(result.CatalogReconciled, Is.True);
                Assert.That(
                    env.LiveSlotDirectoryExists(source.SlotId),
                    Is.False);
                Assert.That(
                    env.TrashRecordExists(result.TrashRecordId),
                    Is.True);
                Assert.That(
                    env.Catalog.Snapshot.TryGetEntry(
                        source.SlotId,
                        out _),
                    Is.False);
            }
        }

        [Test]
        public void TrashPublicationFailureLeavesLiveSlotAuthoritative()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source = env.CreateSource();

                SaveSlotDeletionCoordinator coordinator =
                    env.Coordinator();

                SaveDeletionPlan plan =
                    coordinator.Prepare(source.SlotId);

                env.Backend.FailNextTrashMove();

                SaveSlotDeleteResult result =
                    coordinator.Confirm(plan);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveSlotDeleteStatus.TrashPublicationFailed));
                Assert.That(result.DeleteCommitted, Is.False);
                Assert.That(
                    env.LiveSlotDirectoryExists(source.SlotId),
                    Is.True);
                Assert.That(env.TrashRecordCount(), Is.Zero);
            }
        }

        [Test]
        public void ConsumedPlanCannotBeReplayed()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source = env.CreateSource();

                SaveSlotDeletionCoordinator coordinator =
                    env.Coordinator();

                SaveDeletionPlan plan =
                    coordinator.Prepare(source.SlotId);

                SaveSlotDeleteResult first =
                    coordinator.Confirm(plan);

                Assert.That(first.DeleteCommitted, Is.True);

                int afterFirst = env.Backend.MutationCount;

                SaveSlotDeleteResult replay =
                    coordinator.Confirm(plan);

                Assert.That(
                    replay.Status,
                    Is.EqualTo(SaveSlotDeleteStatus.Consumed));
                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(afterFirst));
            }
        }

        [Test]
        public void ActiveSlotClearsOnlyAfterDurableDelete()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source = env.CreateSource();

                Assert.That(
                    env.Catalog.SelectActiveSlot(
                        source.SlotId).Succeeded,
                    Is.True);

                SaveSlotDeletionCoordinator coordinator =
                    env.Coordinator();

                SaveDeletionPlan plan =
                    coordinator.Prepare(source.SlotId);

                Assert.That(env.Catalog.HasActiveSlot, Is.True);

                SaveSlotDeleteResult result =
                    coordinator.Confirm(plan);

                Assert.That(result.DeleteCommitted, Is.True);
                Assert.That(result.ActiveSlotCleared, Is.True);
                Assert.That(env.Catalog.HasActiveSlot, Is.False);
            }
        }

        [Test]
        public void FailedTrashMoveDoesNotClearActiveSlot()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source = env.CreateSource();

                env.Catalog.SelectActiveSlot(source.SlotId);

                SaveSlotDeletionCoordinator coordinator =
                    env.Coordinator();

                SaveDeletionPlan plan =
                    coordinator.Prepare(source.SlotId);

                env.Backend.FailNextTrashMove();

                SaveSlotDeleteResult result =
                    coordinator.Confirm(plan);

                Assert.That(result.DeleteCommitted, Is.False);
                Assert.That(result.ActiveSlotCleared, Is.False);
                Assert.That(env.Catalog.HasActiveSlot, Is.True);
                Assert.That(
                    env.Catalog.ActiveSlotId,
                    Is.EqualTo(source.SlotId));
            }
        }

        [Test]
        public void DeletingNonActiveSlotPreservesActiveSelection()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var active = env.CreateSource("Active");
                var target = env.CreateSource("Target");

                env.Catalog.SelectActiveSlot(active.SlotId);

                SaveSlotDeletionCoordinator coordinator =
                    env.Coordinator();

                SaveDeletionPlan plan =
                    coordinator.Prepare(target.SlotId);

                SaveSlotDeleteResult result =
                    coordinator.Confirm(plan);

                Assert.That(result.DeleteCommitted, Is.True);
                Assert.That(result.ActiveSlotCleared, Is.False);
                Assert.That(env.Catalog.HasActiveSlot, Is.True);
                Assert.That(
                    env.Catalog.ActiveSlotId,
                    Is.EqualTo(active.SlotId));
            }
        }

        [Test]
        public void CatalogFailureAfterTrashPreservesCommittedDeleteTruth()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source = env.CreateSource();

                SaveSlotDeletionCoordinator coordinator =
                    env.Coordinator();

                SaveDeletionPlan plan =
                    coordinator.Prepare(source.SlotId);

                env.Backend.FailLiveCatalogAfterTrashMove();

                SaveSlotDeleteResult result =
                    coordinator.Confirm(plan);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveSlotDeleteStatus
                            .PublishedCatalogReconciliationFailed));
                Assert.That(result.DeleteCommitted, Is.True);
                Assert.That(result.CatalogReconciled, Is.False);
                Assert.That(
                    env.LiveSlotDirectoryExists(source.SlotId),
                    Is.False);
                Assert.That(
                    env.TrashRecordExists(result.TrashRecordId),
                    Is.True);
            }
        }

        [Test]
        public void TrashRetentionKeepsBoundedNewestRecords()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                SaveSlotDeletionCoordinator coordinator =
                    env.Coordinator(maxTrashRecords: 2);

                for (int i = 0; i < 5; i++)
                {
                    var source =
                        env.CreateSource("Delete " + i);

                    SaveDeletionPlan plan =
                        coordinator.Prepare(source.SlotId);

                    SaveSlotDeleteResult result =
                        coordinator.Confirm(plan);

                    Assert.That(result.DeleteCommitted, Is.True);

                    env.Clock =
                        env.Clock.AddSeconds(1);
                }

                Assert.That(
                    env.TrashRecordCount(),
                    Is.EqualTo(2));
                Assert.That(
                    env.Backend.TrashRetentionDeleteCount,
                    Is.EqualTo(3));
            }
        }

        [Test]
        public void TrashRetentionFailureNeverFabricatesDeleteRollback()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                SaveSlotDeletionCoordinator coordinator =
                    env.Coordinator(maxTrashRecords: 1);

                var first = env.CreateSource("First");

                Assert.That(
                    coordinator.Confirm(
                        coordinator.Prepare(first.SlotId))
                    .DeleteCommitted,
                    Is.True);

                env.Clock =
                    env.Clock.AddSeconds(1);

                var second = env.CreateSource("Second");
                SaveDeletionPlan plan =
                    coordinator.Prepare(second.SlotId);

                env.Backend.FailNextTrashRetentionDelete();

                SaveSlotDeleteResult result =
                    coordinator.Confirm(plan);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveSlotDeleteStatus
                            .PublishedTrashRetentionFailed));
                Assert.That(result.DeleteCommitted, Is.True);
                Assert.That(result.CatalogReconciled, Is.True);
                Assert.That(
                    env.LiveSlotDirectoryExists(second.SlotId),
                    Is.False);
            }
        }

        [Test]
        public void DeletedLiveSlotNoLongerConsumesCanonicalCapacity()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source = env.CreateSource();

                SaveSlotDeletionCoordinator deletion =
                    env.Coordinator();

                Assert.That(
                    deletion.Confirm(
                        deletion.Prepare(source.SlotId))
                    .DeleteCommitted,
                    Is.True);

                SaveTechnicalSlotCreationCoordinator creation =
                    new SaveTechnicalSlotCreationCoordinator(
                        env.Catalog,
                        env.Backend,
                        env.Serializer,
                        env.Integrity,
                        1,
                        4);

                SaveTechnicalSlotCreateResult created =
                    creation.Create(
                        SlotCreationTestEnvironment.Request(
                            "Replacement",
                            "com.example.m410",
                            "1.0.0",
                            "replacement"));

                Assert.That(created.Succeeded, Is.True);
            }
        }

        [Test]
        public void TrashRecordIdentityIsCanonicalAndParseable()
        {
            using (SaveSlotDeletionTestEnvironment env =
                new SaveSlotDeletionTestEnvironment())
            {
                var source = env.CreateSource();

                SaveSlotDeletionCoordinator coordinator =
                    env.Coordinator();

                SaveSlotDeleteResult result =
                    coordinator.Confirm(
                        coordinator.Prepare(source.SlotId));

                Assert.That(result.DeleteCommitted, Is.True);
                Assert.That(
                    SaveTrashRetentionCoordinator.TryParseRecord(
                        result.TrashRecordId,
                        out long ticks),
                    Is.True);
                Assert.That(ticks, Is.GreaterThan(0));
            }
        }
    }
}
