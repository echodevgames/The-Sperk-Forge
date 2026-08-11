
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePublicSlotMutationServiceTests
    {
        [Test]
        public void PublicServiceExposesBoundedRenameAndDuplicateOperations()
        {
            MethodInfo rename =
                typeof(IEchoSaveService).GetMethod(
                    "RenameSlotAsync",
                    new[]
                    {
                        typeof(SaveSlotRenameRequest)
                    });

            MethodInfo duplicate =
                typeof(IEchoSaveService).GetMethod(
                    "DuplicateSlotAsync",
                    new[]
                    {
                        typeof(SaveSlotDuplicateRequest)
                    });

            Assert.That(rename, Is.Not.Null);
            Assert.That(
                rename.ReturnType,
                Is.EqualTo(
                    typeof(Awaitable<SaveSlotRenameResult>)));

            Assert.That(duplicate, Is.Not.Null);
            Assert.That(
                duplicate.ReturnType,
                Is.EqualTo(
                    typeof(Awaitable<SaveSlotDuplicateResult>)));
        }

        [Test]
        public void SlotMutationSurfaceExposesNoPhysicalPathOrDeleteApi()
        {
            Assert.That(
                typeof(SaveSlotRenameRequest).GetProperty("Path"),
                Is.Null);

            Assert.That(
                typeof(SaveSlotDuplicateRequest).GetProperty("Path"),
                Is.Null);

            Assert.That(
                typeof(IEchoSaveService).GetMethod("DeleteSlotAsync"),
                Is.Null);

            Assert.That(
                typeof(IEchoSaveService).GetMethod("TrashSlotAsync"),
                Is.Null);
        }

        [Test]
        public void RenameAndDuplicateBeforeReadyReportServiceNotReady()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                SaveSlotId slot =
                    SaveSlotId.NewId();

                SaveSlotRenameResult rename =
                    env.Service.RenameSlotSynchronouslyForTesting(
                        new SaveSlotRenameRequest(
                            slot,
                            "Before Ready"));

                SaveSlotDuplicateResult duplicate =
                    env.Service.DuplicateSlotSynchronouslyForTesting(
                        new SaveSlotDuplicateRequest(
                            slot));

                Assert.That(
                    rename.Status,
                    Is.EqualTo(
                        SaveSlotRenameStatus.ServiceNotReady));

                Assert.That(
                    duplicate.Status,
                    Is.EqualTo(
                        SaveSlotDuplicateStatus.ServiceNotReady));
            }
        }

        [Test]
        public void OverlappingSlotMutationsReturnBusyAndNeverQueue()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                Assert.That(
                    env.Service.SaveOperationAdmissionForTesting.TryAcquire(
                        out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                SaveSlotRenameResult rename =
                    env.Service.RenameSlotSynchronouslyForTesting(
                        new SaveSlotRenameRequest(
                            env.ActiveSlotId,
                            "Busy"));

                SaveSlotDuplicateResult duplicate =
                    env.Service.DuplicateSlotSynchronouslyForTesting(
                        new SaveSlotDuplicateRequest(
                            env.ActiveSlotId));

                Assert.That(
                    rename.Status,
                    Is.EqualTo(SaveSlotRenameStatus.Busy));

                Assert.That(
                    duplicate.Status,
                    Is.EqualTo(SaveSlotDuplicateStatus.Busy));

                lease.Dispose();
            }
        }

        [Test]
        public void SlotMutationsAfterShutdownReportAdmissionClosed()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                SaveSlotId slot =
                    env.ActiveSlotId;

                Assert.That(
                    env.Service.ShutdownCore().Succeeded,
                    Is.True);

                SaveSlotRenameResult rename =
                    env.Service.RenameSlotSynchronouslyForTesting(
                        new SaveSlotRenameRequest(
                            slot,
                            "Closed"));

                SaveSlotDuplicateResult duplicate =
                    env.Service.DuplicateSlotSynchronouslyForTesting(
                        new SaveSlotDuplicateRequest(
                            slot));

                Assert.That(
                    rename.Status,
                    Is.EqualTo(
                        SaveSlotRenameStatus.AdmissionClosed));

                Assert.That(
                    duplicate.Status,
                    Is.EqualTo(
                        SaveSlotDuplicateStatus.AdmissionClosed));
            }
        }

        [Test]
        public void PublicRenameCommitsWithoutExecutingManualSaveTransaction()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                SaveSlotRenameResult result =
                    env.Service.RenameSlotSynchronouslyForTesting(
                        new SaveSlotRenameRequest(
                            env.ActiveSlotId,
                            "Public Rename"));

                Assert.That(result.HeadPublished, Is.True);
                Assert.That(result.CatalogReconciled, Is.True);
                Assert.That(env.Executor.Calls, Is.Zero);
            }
        }

        [Test]
        public void PublicDuplicateCommitsWithoutManualSaveOrActiveSlotChange()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                SaveSlotId activeBefore =
                    env.ActiveSlotId;

                SaveSlotDuplicateResult result =
                    env.Service.DuplicateSlotSynchronouslyForTesting(
                        new SaveSlotDuplicateRequest(
                            activeBefore));

                Assert.That(result.HeadPublished, Is.True);
                Assert.That(result.CatalogReconciled, Is.True);
                Assert.That(env.Executor.Calls, Is.Zero);

                Assert.That(
                    env.Service.SlotCatalogForTesting.ActiveSlotId,
                    Is.EqualTo(activeBefore));

                Assert.That(
                    result.DuplicateSlotId,
                    Is.Not.EqualTo(activeBefore));
            }
        }
    }
}
