
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePublicCatalogCompositionTests
    {
        [Test]
        public void RegistrationBeforeReadyReportsServiceNotReady()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                PublicRuntimeFacadeParticipant participant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.before");

                SaveParticipantRegistrationResult result =
                    env.Service.RegisterParticipant(
                        participant);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveParticipantRegistrationStatus
                            .ServiceNotReady));

                Assert.That(
                    participant.CaptureCalls,
                    Is.Zero);

                Assert.That(
                    participant.ApplyCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void PublicRegistrationUsesExistingRegistryWithoutCallbacks()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                PublicRuntimeFacadeParticipant participant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.registration",
                        value: 7);

                int mutationsBefore =
                    env.Storage.Backend.MutationCount;

                SaveParticipantRegistrationResult result =
                    env.Service.RegisterParticipant(
                        participant);

                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.Registration, Is.Not.Null);
                Assert.That(result.Registration.IsActive, Is.True);
                Assert.That(
                    participant.CaptureCalls,
                    Is.Zero);
                Assert.That(
                    participant.ApplyCalls,
                    Is.Zero);
                Assert.That(
                    env.Storage.Backend.MutationCount,
                    Is.EqualTo(mutationsBefore));
                Assert.That(
                    env.Service.ParticipantRegistryForTesting.Count,
                    Is.EqualTo(1));
            }
        }

        [Test]
        public void DuplicateAndAliasCollisionsRemainRejected()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                SaveParticipantRegistrationResult first =
                    env.Service.RegisterParticipant(
                        new PublicRuntimeFacadeParticipant(
                            "com.example.r1.owner",
                            aliases:
                                new[]
                                {
                                    "com.example.r1.legacy"
                                }));

                Assert.That(first.Succeeded, Is.True);

                SaveParticipantRegistrationResult duplicate =
                    env.Service.RegisterParticipant(
                        new PublicRuntimeFacadeParticipant(
                            "com.example.r1.owner"));

                SaveParticipantRegistrationResult aliasCollision =
                    env.Service.RegisterParticipant(
                        new PublicRuntimeFacadeParticipant(
                            "com.example.r1.legacy"));

                Assert.That(
                    duplicate.Status,
                    Is.EqualTo(
                        SaveParticipantRegistrationStatus
                            .DuplicateId));

                Assert.That(
                    aliasCollision.Status,
                    Is.EqualTo(
                        SaveParticipantRegistrationStatus
                            .AliasCollision));
            }
        }

        [Test]
        public void RegistrationDisposalReleasesExactOwnership()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                PublicRuntimeFacadeParticipant firstParticipant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.release");

                SaveParticipantRegistrationResult first =
                    env.Service.RegisterParticipant(
                        firstParticipant);

                Assert.That(first.Succeeded, Is.True);

                first.Registration.Dispose();

                Assert.That(
                    first.Registration.IsActive,
                    Is.False);

                SaveParticipantRegistrationResult replacement =
                    env.Service.RegisterParticipant(
                        new PublicRuntimeFacadeParticipant(
                            "com.example.r1.release"));

                Assert.That(replacement.Succeeded, Is.True);

                first.Registration.Dispose();

                Assert.That(
                    replacement.Registration.IsActive,
                    Is.True);
            }
        }

        [Test]
        public void ShutdownInvalidatesServiceOwnedRegistrations()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                SaveParticipantRegistrationResult registered =
                    env.Service.RegisterParticipant(
                        new PublicRuntimeFacadeParticipant(
                            "com.example.r1.shutdown"));

                Assert.That(registered.Succeeded, Is.True);
                Assert.That(registered.Registration.IsActive, Is.True);

                Assert.That(
                    env.Service.ShutdownCore().Succeeded,
                    Is.True);

                Assert.That(
                    registered.Registration.IsActive,
                    Is.False);

                SaveParticipantRegistrationResult afterShutdown =
                    env.Service.RegisterParticipant(
                        new PublicRuntimeFacadeParticipant(
                            "com.example.r1.aftershutdown"));

                Assert.That(
                    afterShutdown.Status,
                    Is.EqualTo(
                        SaveParticipantRegistrationStatus
                            .AdmissionClosed));
            }
        }

        [Test]
        public void CatalogSnapshotIsMemoryOnlyAndStartsEmpty()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                int mutationsBefore =
                    env.Storage.Backend.MutationCount;

                SaveSlotCatalogSnapshot snapshot =
                    env.Service.GetCatalogSnapshot();

                Assert.That(snapshot, Is.Not.Null);
                Assert.That(snapshot.Count, Is.Zero);
                Assert.That(
                    env.Storage.Backend.MutationCount,
                    Is.EqualTo(mutationsBefore));
            }
        }

        [Test]
        public void PublicCreateReusesTechnicalPublicationAndDoesNotAutoSelect()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                SaveSlotCreateResult created =
                    env.Service
                        .CreateSlotSynchronouslyForTesting(
                            env.CreateRequest(
                                "R1 Created"));

                Assert.That(created.Succeeded, Is.True);
                Assert.That(created.SlotPublished, Is.True);
                Assert.That(created.CatalogReconciled, Is.True);
                Assert.That(created.CreatedEntry, Is.Not.Null);
                Assert.That(
                    created.CreatedEntry.DisplayName,
                    Is.EqualTo("R1 Created"));
                Assert.That(
                    env.Service.GetCatalogSnapshot().Count,
                    Is.EqualTo(1));
                Assert.That(
                    env.Service.SlotCatalogForTesting.HasActiveSlot,
                    Is.False);
            }
        }

        [Test]
        public void PublicSelectIsSessionOnlyAndRejectsUnknownSlot()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                SaveSlotCreateResult created =
                    env.CreateSlot();

                int mutationsBeforeSelection =
                    env.Storage.Backend.MutationCount;

                SaveActiveSlotSelectionResult selected =
                    env.Service
                        .SelectSlotSynchronouslyForTesting(
                            created.SlotId);

                Assert.That(selected.Succeeded, Is.True);
                Assert.That(selected.HasActiveSlot, Is.True);
                Assert.That(
                    selected.ActiveSlotId,
                    Is.EqualTo(created.SlotId));
                Assert.That(
                    env.Storage.Backend.MutationCount,
                    Is.EqualTo(mutationsBeforeSelection));

                SaveActiveSlotSelectionResult rejected =
                    env.Service
                        .SelectSlotSynchronouslyForTesting(
                            SaveSlotId.NewId());

                Assert.That(
                    rejected.Status,
                    Is.EqualTo(
                        SaveActiveSlotSelectionStatus.Rejected));
                Assert.That(
                    env.Service.SlotCatalogForTesting.ActiveSlotId,
                    Is.EqualTo(created.SlotId));
            }
        }

        [Test]
        public void PublicCatalogRefreshReturnsBusyAndNeverQueues()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);
                env.CreateSlot();

                Assert.That(
                    env.Service.SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                SaveSlotCatalogRefreshResult busy =
                    env.Service
                        .RefreshCatalogSynchronouslyForTesting();

                Assert.That(
                    busy.Status,
                    Is.EqualTo(
                        SaveSlotCatalogRefreshStatus.Busy));

                lease.Dispose();

                SaveSlotCatalogRefreshResult retry =
                    env.Service
                        .RefreshCatalogSynchronouslyForTesting();

                Assert.That(retry.Succeeded, Is.True);
                Assert.That(retry.Snapshot.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void PublicSlotCreateReturnsBusyAndNeverQueues()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                Assert.That(
                    env.Service.SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                SaveSlotCreateResult busy =
                    env.Service
                        .CreateSlotSynchronouslyForTesting(
                            env.CreateRequest());

                Assert.That(
                    busy.Status,
                    Is.EqualTo(
                        SaveSlotCreateStatus.Busy));

                Assert.That(busy.SlotPublished, Is.False);

                lease.Dispose();

                SaveSlotCreateResult retry =
                    env.Service
                        .CreateSlotSynchronouslyForTesting(
                            env.CreateRequest());

                Assert.That(retry.Succeeded, Is.True);
            }
        }

        [Test]
        public void R1RetainsTechnicalCapacityOfSixtyFour()
        {
            Assert.That(
                EchoSaveService.DefaultTechnicalSlotCapacity,
                Is.EqualTo(64));
        }
    }
}
