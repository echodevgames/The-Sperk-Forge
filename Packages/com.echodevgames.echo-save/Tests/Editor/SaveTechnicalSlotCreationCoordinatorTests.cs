
using System;
using System.IO;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveTechnicalSlotCreationCoordinatorTests
    {
        [Test]
        public void ZeroOrNegativeCapacityIsRejected()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                Assert.Throws<ArgumentOutOfRangeException>(
                    () =>
                        new SaveTechnicalSlotCreationCoordinator(
                            env.Catalog,
                            env.CreatePublicationCoordinator(),
                            0,
                            1,
                            SaveSlotId.NewId));

                Assert.Throws<ArgumentOutOfRangeException>(
                    () =>
                        new SaveTechnicalSlotCreationCoordinator(
                            env.Catalog,
                            env.CreatePublicationCoordinator(),
                            -1,
                            1,
                            SaveSlotId.NewId));
            }
        }

        [Test]
        public void ZeroOrNegativeCollisionAttemptBoundIsRejected()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                Assert.Throws<ArgumentOutOfRangeException>(
                    () =>
                        new SaveTechnicalSlotCreationCoordinator(
                            env.Catalog,
                            env.CreatePublicationCoordinator(),
                            8,
                            0,
                            SaveSlotId.NewId));

                Assert.Throws<ArgumentOutOfRangeException>(
                    () =>
                        new SaveTechnicalSlotCreationCoordinator(
                            env.Catalog,
                            env.CreatePublicationCoordinator(),
                            8,
                            -1,
                            SaveSlotId.NewId));
            }
        }

        [Test]
        public void OverlongRequestFailsBeforeDurableMutation()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                SaveTechnicalSlotCreationCoordinator coordinator =
                    env.CreateSlotCoordinator(
                        8,
                        2,
                        SaveSlotId.NewId);

                SaveTechnicalSlotCreateResult result =
                    coordinator.Create(
                        SlotCreationTestEnvironment.Request(
                            new string(
                                'x',
                                SaveTechnicalSlotCreationCoordinator
                                    .MaximumMetadataTextLength + 1)));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveTechnicalSlotCreateStatus
                            .InvalidRequest));

                Assert.That(
                    env.Backend.MutationCount,
                    Is.Zero);
            }
        }

        [Test]
        public void CatalogFailureStopsCreateBeforeDurableMutation()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                env.Backend.FailDiscoveryAfterHeadPublication =
                    false;

                SaveStorageKey.TryCreate(
                    "slots",
                    out SaveStorageKey slotsKey);

                // Force the local discovery provider to remain valid, then
                // wrap it through a backend that is already faulted by marking
                // a synthetic head publication state via one real publication.
                SaveSlotId seed =
                    SaveSlotId.NewId();

                Assert.That(
                    env.CreatePublicationCoordinator()
                        .PublishInitialEmptyTransportGeneration(
                            seed,
                            "seed",
                            "1",
                            "a",
                            "Seed")
                        .Succeeded,
                    Is.True);

                int mutationsBefore =
                    env.Backend.MutationCount;

                env.Backend.FailDiscoveryAfterHeadPublication =
                    true;

                SaveTechnicalSlotCreateResult result =
                    env.CreateSlotCoordinator(
                            8,
                            2,
                            SaveSlotId.NewId)
                        .Create(
                            SlotCreationTestEnvironment.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveTechnicalSlotCreateStatus
                            .CatalogUnavailable));

                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(
                        mutationsBefore));
            }
        }

        [Test]
        public void DegradedCanonicalSlotCountsAgainstCapacity()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                SaveSlotId degraded =
                    SaveSlotId.NewId();

                env.CreateRawSlotDirectory(
                    degraded.Value);

                int mutationsBefore =
                    env.Backend.MutationCount;

                SaveTechnicalSlotCreateResult result =
                    env.CreateSlotCoordinator(
                            1,
                            2,
                            SaveSlotId.NewId)
                        .Create(
                            SlotCreationTestEnvironment.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveTechnicalSlotCreateStatus
                            .CapacityReached));

                Assert.That(
                    env.Catalog.Snapshot.Count,
                    Is.EqualTo(1));

                Assert.That(
                    env.Catalog.Snapshot.DegradedCount,
                    Is.EqualTo(1));

                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(
                        mutationsBefore));
            }
        }

        [Test]
        public void InvalidChildDirectoryDoesNotConsumeCapacity()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                env.CreateRawSlotDirectory(
                    "not-a-slot");

                SaveTechnicalSlotCreateResult result =
                    env.CreateSlotCoordinator(
                            1,
                            2,
                            SaveSlotId.NewId)
                        .Create(
                            SlotCreationTestEnvironment.Request());

                Assert.That(
                    result.Succeeded,
                    Is.True);

                Assert.That(
                    env.Catalog.Snapshot.Count,
                    Is.EqualTo(1));
            }
        }

        [Test]
        public void GeneratedSlotIdCollisionRetriesAndUsesFreshId()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                SaveSlotId existing =
                    SaveSlotId.NewId();

                Assert.That(
                    env.CreatePublicationCoordinator()
                        .PublishInitialEmptyTransportGeneration(
                            existing,
                            "game",
                            "1",
                            "a",
                            "Existing")
                        .Succeeded,
                    Is.True);

                SaveSlotId fresh =
                    SaveSlotId.NewId();

                int calls = 0;

                SaveSlotId Factory()
                {
                    calls++;

                    return calls == 1
                        ? existing
                        : fresh;
                }

                SaveTechnicalSlotCreateResult result =
                    env.CreateSlotCoordinator(
                            4,
                            2,
                            Factory)
                        .Create(
                            SlotCreationTestEnvironment.Request());

                Assert.That(
                    result.Succeeded,
                    Is.True);

                Assert.That(
                    calls,
                    Is.EqualTo(2));

                Assert.That(
                    result.SlotId,
                    Is.EqualTo(
                        fresh));
            }
        }

        [Test]
        public void CollisionRetryExhaustionFailsBeforePublicationMutation()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                SaveSlotId existing =
                    SaveSlotId.NewId();

                Assert.That(
                    env.CreatePublicationCoordinator()
                        .PublishInitialEmptyTransportGeneration(
                            existing,
                            "game",
                            "1",
                            "a",
                            "Existing")
                        .Succeeded,
                    Is.True);

                int mutationsBefore =
                    env.Backend.MutationCount;

                SaveTechnicalSlotCreateResult result =
                    env.CreateSlotCoordinator(
                            4,
                            2,
                            () => existing)
                        .Create(
                            SlotCreationTestEnvironment.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveTechnicalSlotCreateStatus
                            .SlotIdCollisionLimitExceeded));

                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(
                        mutationsBefore));
            }
        }

        [Test]
        public void InvalidGeneratedSlotIdFailsBeforePublicationMutation()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                int mutationsBefore =
                    env.Backend.MutationCount;

                SaveTechnicalSlotCreateResult result =
                    env.CreateSlotCoordinator(
                            4,
                            2,
                            () => default)
                        .Create(
                            SlotCreationTestEnvironment.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveTechnicalSlotCreateStatus
                            .SlotIdGenerationFailed));

                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(
                        mutationsBefore));
            }
        }

        [Test]
        public void DisplayNameIsMetadataOnlyAndNeverBecomesAPath()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                const string displayName =
                    "../PLAYER DISPLAY / NOT A PATH";

                SaveTechnicalSlotCreateResult result =
                    env.CreateSlotCoordinator(
                            4,
                            2,
                            SaveSlotId.NewId)
                        .Create(
                            SlotCreationTestEnvironment.Request(
                                displayName));

                Assert.That(
                    result.Succeeded,
                    Is.True);

                Assert.That(
                    result.CreatedEntry.DisplayName,
                    Is.EqualTo(
                        displayName));

                Assert.That(
                    Directory.Exists(
                        Path.Combine(
                            env.Local.RootPath,
                            "slots",
                            displayName)),
                    Is.False);
            }
        }

        [Test]
        public void SuccessfulCreationPublishesEmptyGenerationAndReconcilesCatalog()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                SaveSlotId chosen =
                    SaveSlotId.NewId();

                SaveTechnicalSlotCreateResult result =
                    env.CreateSlotCoordinator(
                            4,
                            2,
                            () => chosen)
                        .Create(
                            SlotCreationTestEnvironment.Request(
                                "Created Save"));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveTechnicalSlotCreateStatus.Succeeded));

                Assert.That(
                    result.SlotPublished,
                    Is.True);

                Assert.That(
                    result.CatalogReconciled,
                    Is.True);

                Assert.That(
                    result.SlotId,
                    Is.EqualTo(
                        chosen));

                Assert.That(
                    result.CreatedEntry,
                    Is.Not.Null);

                Assert.That(
                    result.CreatedEntry.IsSelectable,
                    Is.True);

                Assert.That(
                    result.CreatedEntry.CurrentGenerationId,
                    Is.EqualTo(
                        result.GenerationId));

                Assert.That(
                    env.Catalog.HasActiveSlot,
                    Is.False);

                Assert.That(
                    env.ReadPayload(
                            result.SlotId,
                            result.GenerationId)
                        .entries,
                    Is.Empty);

                Assert.That(
                    env.ReadManifest(
                            result.SlotId,
                            result.GenerationId)
                        .payloadEntries,
                    Is.Empty);
            }
        }

        [Test]
        public void ExistingHeadRaceFailsClosedInsteadOfUpdatingSlot()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                SaveSlotId raced =
                    SaveSlotId.NewId();

                // Preflight snapshot is empty.
                Assert.That(
                    env.Catalog.Refresh()
                        .Succeeded,
                    Is.True);

                // Simulate another writer creating the chosen technical slot
                // before this create transaction reaches its in-transaction
                // existing-head check.
                Assert.That(
                    env.CreatePublicationCoordinator()
                        .PublishInitialEmptyTransportGeneration(
                            raced,
                            "other",
                            "1",
                            "a",
                            "Other")
                        .Succeeded,
                    Is.True);

                int mutationsBefore =
                    env.Backend.MutationCount;

                SaveTechnicalSlotCreateResult result =
                    env.CreateSlotCoordinator(
                            8,
                            1,
                            () => raced)
                        .Create(
                            SlotCreationTestEnvironment.Request());

                // The coordinator refreshes again before ID allocation, so the
                // raced slot is visible and collision exhaustion occurs before
                // publication mutation. Direct in-transaction existing-head
                // rejection is covered separately above.
                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveTechnicalSlotCreateStatus
                            .SlotIdCollisionLimitExceeded));

                Assert.That(
                    env.Backend.MutationCount,
                    Is.EqualTo(
                        mutationsBefore));
            }
        }

        [Test]
        public void PublicationFailureDoesNotFabricateCreatedCatalogEntry()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                env.Backend.Fault =
                    SlotCreationFaultPoint.HeadPublication;

                SaveTechnicalSlotCreateResult result =
                    env.CreateSlotCoordinator(
                            4,
                            2,
                            SaveSlotId.NewId)
                        .Create(
                            SlotCreationTestEnvironment.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveTechnicalSlotCreateStatus
                            .PublicationFailed));

                Assert.That(
                    result.SlotPublished,
                    Is.False);

                Assert.That(
                    result.CatalogReconciled,
                    Is.False);

                Assert.That(
                    result.CreatedEntry,
                    Is.Null);

                Assert.That(
                    env.Catalog.HasActiveSlot,
                    Is.False);
            }
        }

        [Test]
        public void PostPublicationCatalogFailureReportsDurableTruth()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                env.Backend.FailDiscoveryAfterHeadPublication =
                    true;

                SaveSlotId chosen =
                    SaveSlotId.NewId();

                SaveTechnicalSlotCreateResult result =
                    env.CreateSlotCoordinator(
                            4,
                            2,
                            () => chosen)
                        .Create(
                            SlotCreationTestEnvironment.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveTechnicalSlotCreateStatus
                            .PublishedCatalogReconciliationFailed));

                Assert.That(
                    result.SlotPublished,
                    Is.True);

                Assert.That(
                    result.CatalogReconciled,
                    Is.False);

                Assert.That(
                    result.CreatedEntry,
                    Is.Null);

                Assert.That(
                    env.ReadHead(
                            chosen)
                        .Succeeded,
                    Is.True);

                Assert.That(
                    env.Catalog.HasActiveSlot,
                    Is.False);
            }
        }
    }
}
